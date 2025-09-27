import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { fabric } from 'fabric';
import { Constants } from '../common/constants.model';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { WidgetType } from '../shared/models/widget-type.enum';
import { CanvasItem } from '../models/canvas-item.model';
import { FormsModule } from '@angular/forms';
import { Canvas } from '../models/canvas.model';
import { TemplateNew } from '../shared/models/templatenew.modal';
import { Observable } from 'rxjs';
import jsPDF from "jspdf";
import { coords } from '../models/point.model';

@Component({
  selector: 'app-canvas-editor',
  standalone: true,
  templateUrl: './canvas-editor.component.html',
  styleUrls: ['./canvas-editor.component.css'],
  imports: [
              CommonModule,
              HttpClientModule,
              FormsModule
           ]
})
export class CanvasEditorComponent {

    @ViewChild('fabricCanvas', { static: true }) fabricCanvasRef!: ElementRef<HTMLCanvasElement>;
  @Output() itemSelected = new EventEmitter<any>();
  @Output() canvasChanged = new EventEmitter<string>();  // emit preview URL
   @Input() templateId = '';
  @Input() templateSvg = '';
  @Input() fabricJson = '';
  @Input() templateVersionId = '';

    canvas!: fabric.Canvas;
    idCounters: number = 0;
    canvasItem: Canvas= {id: 'canvas1', height: 400, width: 800, bgColor: '#f9fafb', type: 'canvas'};
 

    constructor(private http: HttpClient) {
        this.extendFabricObject();     
    }

    private makeDotPattern(dotSize = 1, spacing = 10, color = '#e0e0e0') {
      const patternCanvas = document.createElement('canvas');
      const ctx = patternCanvas.getContext('2d');

      patternCanvas.width = spacing;
      patternCanvas.height = spacing;

      ctx.fillStyle = color;
      ctx.beginPath();
      ctx.arc(dotSize, dotSize, dotSize, 0, 2 * Math.PI);
      ctx.fill();

      return new fabric.Pattern({
        source: patternCanvas.toDataURL() as any,
        repeat: 'repeat'
      });
  }

  private extendFabricObject() {
    fabric.Object.prototype.toObject = (function (toObject) {
      return function (this: fabric.Object, propertiesToInclude?: string[]) {
        return toObject.call(this, (propertiesToInclude || []).concat([
          'IsDynamic',
          'Id',
          'CustomType',
          'BarcodeType'
        ]));
      };
    })(fabric.Object.prototype.toObject);
  }

    private extendDeleteOnAllObjects()
    {
        // Add custom delete control globally
        (fabric.Object.prototype as any).controls.deleteControl = new fabric.Control({
          x: 0.5,          // position: top-right
          y: -0.5,
          offsetX: 10,
          offsetY: -10,
          cursorStyle: 'pointer',
          mouseUpHandler: (eventData, transform) => {
            const target = transform.target;
            const canvas = target.canvas;
            canvas.remove(target);             // delete the object
            canvas.requestRenderAll();
            this.onMouseDownCanvas();
            return true;
          },
          render: (ctx, left, top, styleOverride, fabricObject) => {
            const size = 20;
            ctx.save();
            ctx.translate(left, top);
            // Draw red circle
            ctx.fillStyle = 'red';
            ctx.strokeStyle = 'white';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(0, 0, size / 2, 0, Math.PI * 2);
            ctx.fill();
            ctx.stroke();
            // Draw white '×'
            ctx.fillStyle = 'white';
            ctx.font = 'bold 14px Arial';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('×', 0, 0);
            ctx.restore();
          }
        });
    }

    updateCanvas(item: CanvasItem) {

        this.canvasItem = {height: item.height, width: item.width, bgColor: item.bgColor, id: item.id, type: item.type};
        this.canvas.setWidth(item.width);
        this.canvas.setHeight(item.height);
        this.canvas.setBackgroundColor(item.bgColor, undefined);
        this.canvas.renderAll();
    }
  
    ngAfterViewInit() {

      this.canvas = new fabric.Canvas(this.fabricCanvasRef.nativeElement, {
        backgroundColor: this.canvasItem.bgColor,
        selection: true,
        width: this.canvasItem.width,
        height: this.canvasItem.height,
        preserveObjectStacking: true,
        
      });

      //this.canvas.setBackgroundColor(this.makeDotPattern(), undefined);

      this.extendDeleteOnAllObjects();
  
      // Listen for selection
      this.canvas.on('selection:created', (e) => this.onSelection(e));
      this.canvas.on('selection:updated', (e) => this.onSelection(e));

      // Show controls when canvas is clicked
      this.canvas.on('mouse:down', (e) => {
         if (!e.target) {
            this.onCanvasSelection(e);
         }
      });

      setTimeout(()=>{

        this.onMouseDownCanvas();
      }, 500);
      
  
      // Deselect event
      this.canvas.on('selection:cleared', () => this.itemSelected.emit(null));
 

      // load initial content if any
      // e.g. this.canvas.loadFromJSON(...)

      // Listen to changes
      this.canvas.on('object:added', () => this.emitPreview());
      this.canvas.on('object:modified', () => {
        
        this.emitPreview();
      });
      
      this.canvas.on('object:removed', () => this.emitPreview());
      // you may also listen to selection:cleared etc.

      // Emit initial preview
      this.emitPreview();
      // register grouping events
    //this.registerGroupingEvents();

     if (this.templateId.length > 0) {
      
       this.loadCanvasFromJSON(this.fabricJson);
      }
    }

    private onMouseDownCanvas(){

      this.canvas.fire('mouse:down', {
          e: {}, // fake mouse event
          pointer: { x: 0, y: 0 } // coordinates (optional)
        });
    }

    private bringAllRequiredObjectsToFront() {

      this.canvas.getObjects().forEach((obj) => {
        if ((obj.type === 'textbox' || obj instanceof fabric.Textbox) ||
            (obj.type === 'image' || obj instanceof fabric.Image)) {
          obj.bringToFront();
        }
      });
      this.canvas.renderAll();
    }

  private registerGroupingEvents() {
    // ungroup logic
    this.ungroupObjects()

    // Grouping logic
    this.groupAlignedObjects();
  }

  private groupAlignedObjects() {

    this.canvas.on('mouse:up', (e) => {
      const activeObj = e.target as fabric.Object;
      if (!activeObj) return;

      this.canvas.forEachObject((obj) => {
        if (obj === activeObj) return;

        if (activeObj.intersectsWithObject(obj)) {
          this.canvas.remove(activeObj);
          this.canvas.remove(obj);

          const group = new fabric.Group([activeObj, obj], {
            left: Math.min(activeObj.left ?? 0, obj.left ?? 0),
            top: Math.min(activeObj.top ?? 0, obj.top ?? 0)
          });

          this.canvas.add(group);
          this.canvas.setActiveObject(group);
          this.canvas.renderAll();
        }
      });
    });
  }

  private ungroupObjects() {
    this.canvas.on('mouse:dblclick', (opt) => {
      const target = opt.target as fabric.Object;
      if (!target || target.type !== 'group') return;

      const group = target as fabric.Group;
      const objects = group._objects;

      // Restore original state of objects
      group._restoreObjectsState();

      // Remove the group
      this.canvas.remove(group);

      // Add all child objects back to canvas
      objects.forEach(obj => this.canvas.add(obj));

      this.canvas.renderAll();
    });
  }

    private enableDelete(obj: fabric.Object) {
        (obj as any).setControlsVisibility({
          mt: true, mb: true, ml: true, mr: true, tl: true, tr: true, bl: true, br: true,
          deleteControl: true // show the delete button
        });
      }

    private onCanvasSelection(e:any){

      if (!e.target) {
        // Map Fabric object to CanvasItem structure
        const item: CanvasItem ={
            id: 'mainCanvas',
            type: this.canvasItem.type,
            bgColor: this.canvasItem.bgColor,
            height: this.canvasItem.height,
            width: this.canvasItem.width
          };
        
          this.itemSelected.emit(item); // send to AppComponent
      }
    }
    
    private getBgColor(obj: any): string{
        
        let color: string = '';
        if(obj.type === 'line') color = obj.stroke;
        else{

          color = obj.fill;
        }
        
        return color === 'none' || color === '' ? '#ffffff' : color;
    }
    onSelection(e: any) {
      const obj = e.selected?.[0]; // single selection
      if (!obj) return;
  
      // Map Fabric object to CanvasItem structure
      const item: CanvasItem ={
      id: obj.Id,
      type: obj.CustomType,
      label: obj.CustomType === 'textbox' ? obj.text : '',
      color: obj.fill === 'none' || obj.fill === '' ? '#ffffff' : obj.fill,
      bgColor: obj.fill === 'none' || obj.fill === '' ? '#ffffff' : obj.fill,
      fontSize: obj.fontSize,
      bold: obj.fontWeight === 'bold',
      italic: obj.fontStyle === 'italic',
      underline: obj.underline,
      src: obj.getSrc ? obj.getSrc() : obj.src || null,
      isDynamic: obj.IsDynamic,
      barcodeType: obj.BarcodeType,
      linethrough: obj.linethrough,
      stroke: obj.stroke,
      strokeWidth: obj.strokeWidth,
      left: obj.left,
      top: obj.top,
      fabricObject: obj
    };
  
      this.itemSelected.emit(item); // send to AppComponent
    }
  
    updateItem(updatedItem: any) {
        const obj = updatedItem.fabricObject;
        //if (!obj) return;
        let left: number = updatedItem.left && updatedItem.left > 0 ? updatedItem.left : obj.left;
        let top: number = updatedItem.top && updatedItem.top > 0 ? updatedItem.top : obj.top;

        // update left, top for all the widgets.
        if(updatedItem.type !== 'canvas')
        {
          obj.set({
            left: left,
            top: top
          });
        }

        // update other properties case basis.
        switch (updatedItem.type) {
          case "textbox":
            obj.set({
              text: updatedItem.label,
              fill: updatedItem.color,
              fontSize: updatedItem.fontSize,
              fontWeight: updatedItem.bold ? 'bold' : 'normal',
              fontStyle: updatedItem.italic ? 'italic' : 'normal',
              underline: updatedItem.underline || false,
              linethrough: updatedItem.linethrough || false
            });
            obj.IsDynamic = updatedItem.isDynamic;
            break;
  
          case "image":
            if (updatedItem.src) {
                let imgId = obj.Id;
                fabric.Image.fromURL(updatedItem.src, (img) => {
                  img.set({
                    left: obj.left,
                    top: obj.top,
                    scaleX: obj.scaleX,
                    scaleY: obj.scaleY
                  });
                  img.IsDynamic = false;
                  img.CustomType = 'image';
                  img.Id = imgId;
                  this.canvas.remove(obj);
                  this.canvas.add(img);
                  
                  this.canvas.setActiveObject(img);
                  this.canvas.requestRenderAll();
                });
              }
            break;
          case "canvas":
              this.updateCanvas(updatedItem);
              break;
          
          case "barcode":
            obj.BarcodeType = updatedItem.barcodeType;
            
            break;
          case "circle":
          case "ploygon":
          case "rect":
              obj.set({
                        stroke: updatedItem.stroke,
                        strokeWidth: updatedItem.strokeWidth,
                        fill: updatedItem.bgColor
                      });
              break;
          case "line":
              obj.set({
                        strokeWidth: updatedItem.strokeWidth,
                      });
              break;
  
          default: // shapes
            obj.set({ fill: updatedItem.bgColor });
        }
  
        this.canvas.requestRenderAll();
      }
  
    // Must prevent default to allow drop
    onDragOver(event: DragEvent) {
      event.preventDefault();
    }
    
    // Drop handler
    onDrop(event: DragEvent) {
      event.preventDefault();
  
      // Read item data from Toolbox
      const data = event.dataTransfer?.getData('application/json');
      if (!data) return;
  
      const item = JSON.parse(data);
  
      this.drawShape(item, event.offsetX, event.offsetY);
    }

    drawShape(item: any, offsetX: number, offsetY: number){

      // Draw Fabric shapes based on item.type
      switch (item.type) {
        case WidgetType.Circle:
          this.addCircle(offsetX, offsetY);
          break;
        case WidgetType.Line:
          this.addLine(offsetX, offsetY);
          break;
        case WidgetType.Triangle:
          this.addTriangle(offsetX, offsetY);
          break;
        case WidgetType.Rectangle:
          this.addRectangle(offsetX, offsetY);
          break;
        case WidgetType.Barcode:
          this.addBarcode(offsetX, offsetY);
          break;
        case WidgetType.Text:
          this.addTextbox(offsetX, offsetY);
          break;
        case WidgetType.Image:
          this.addImage(offsetX, offsetY);
          break;
        case WidgetType.Custom:
          this.addCustomTemplate(offsetX, offsetY);
          break;
  
        // Add more types later if needed
        default:
          console.log('Dropped item type not handled:', item.type);
      }
    }

  private addRectangle(x: number, y: number) {
    fabric.loadSVGFromString(Constants.SVGTemplate.Rectangle, (objects, options) => {
      const rectangle = fabric.util.groupSVGElements(objects, options);

      rectangle.set({
        left: x,
        top: y,
        selectable: true,
        hasControls: true,
        hasBorders: true,
        strokeUniform: true
      });

      rectangle.Id = this.generateId(WidgetType.Rectangle);
      rectangle.IsDynamic = false;
      rectangle.CustomType = 'rect';

      this.canvas.add(rectangle);
      this.bringAllRequiredObjectsToFront();
      this.canvas.setActiveObject(rectangle);
      this.canvas.requestRenderAll();

      // Auto-select for properties panel
      this.onSelection({ selected: [rectangle] });
    });
  }

  private preprocessFabricJson(fabricJson: any) {
      if (!fabricJson.objects) return fabricJson;

      fabricJson.objects.forEach((obj: any) => {
        // Map CustomType to Fabric-recognized type
        if (obj.CustomType) {
          switch (obj.CustomType.toLowerCase()) {
            case 'rect':
              obj.type = 'rect';
              break;
            case 'textbox':
              obj.type = 'textbox';
              break;
            case 'circle':
              obj.type = 'circle';
              break;
            case 'line':
              obj.type = 'line';
              break;
            case 'polygon':
              obj.type = 'polygon';
              break;
            case 'image':
              obj.type = 'image';
              break;
            // case 'barcode':
            //   // Barcode is custom, map to image or implement custom Fabric class
            //   obj.type = 'image';
              break;
            case 'group':
              obj.type = 'group';
              break;
            default:
              obj.type = 'object';
          }
        }
      });

      return fabricJson;
    }


  private makeObjectsSelectable(canvas: fabric.Canvas) {
    const objects = canvas.getObjects();
    
    for (let i = objects.length - 1; i >= 0; i--) {
        const obj = objects[i];

        if (obj.type === 'group' || obj.type === 'activeSelection') {
            const group = obj as fabric.Group;
            const items = group._objects.slice(); // clone objects
            canvas.remove(group); // remove the group

            items.forEach(item => {
                item.set({
                    left: item.left! + group.left!,
                    top: item.top! + group.top!,
                    selectable: true
                });
                canvas.add(item); // add each object individually
            });
        } else {
            obj.set({ selectable: true });
        }
    }

    canvas.renderAll();
}

  private addCustomTemplate(x: number, y: number) {

    var json = this.preprocessFabricJson(JSON.stringify(Constants.SVGTemplate.Custom1));
    this.canvas.loadFromJSON(json, () => {
      this.makeObjectsSelectable(this.canvas);

      this.canvas.renderAll();
    });

  }

  private addImage(x: number, y: number) {
    fabric.loadSVGFromString(Constants.SVGTemplate.Image, (objects, options) => {
      const imgGroup = fabric.util.groupSVGElements(objects, options);

      imgGroup.set({
        left: x,
        top: y,
        selectable: true,
        hasControls: true,
        hasBorders: true
      });

      imgGroup.Id = this.generateId(WidgetType.Image);
      imgGroup.IsDynamic = false;
      imgGroup.CustomType = 'image';

      this.canvas.add(imgGroup);
      this.bringAllRequiredObjectsToFront();
      this.canvas.setActiveObject(imgGroup);
      this.canvas.requestRenderAll();

      this.onSelection({ selected: [imgGroup] });
    });
  }
  private addTextbox(x: number, y: number) {
    const textbox = new fabric.Textbox('Sample Text', {
      left: x,
      top: y,
      fontSize: 18,
      fontFamily: 'Arial',
      fill: 'black',
      editable: true,
      borderColor: 'black',
      cornerColor: 'black',
      cornerSize: 6,
      transparentCorners: false,
      linethrough: false 
    });

    textbox.IsDynamic = false;
    textbox.Id = this.generateId(WidgetType.Text);
    textbox.CustomType = 'textbox';

    (textbox as any).controls.deleteControl = (fabric.Object.prototype as any).controls.deleteControl;

    // Enable delete button for this textbox
    (textbox as any).setControlsVisibility({
      tl: true, tr: true, bl: true, br: true, mt: true, mb: true, ml: true, mr: true, mtr: true,
      deleteControl: true
    });

    // Hide delete button while editing
    textbox.on('editing:entered', () => {
      (textbox as any).setControlsVisibility({ deleteControl: false });
      this.canvas.requestRenderAll();
    });

    // Show delete button when editing ends
    textbox.on('editing:exited', () => {
      (textbox as any).setControlsVisibility({ deleteControl: true });
      this.canvas.requestRenderAll();
    });

    this.canvas.add(textbox);
    this.bringAllRequiredObjectsToFront();
    this.canvas.setActiveObject(textbox);
    this.canvas.requestRenderAll();

    // Auto-select the label for your properties panel
    this.onSelection({ selected: [textbox] });
  }

  private addBarcode(x: number, y: number) {
    fabric.loadSVGFromString(Constants.SVGTemplate.Barcode, (objects, options) => {
      const barcode = fabric.util.groupSVGElements(objects, options);

      barcode.set({
        type : "image",
        left: x,
        top: y,
        selectable: true,
        hasControls: true,
        hasBorders: true,
      });

      barcode.Id = this.generateId(WidgetType.Barcode);
      barcode.IsDynamic = false;
      barcode.CustomType = 'barcode';
      barcode.BarcodeType = 'Code128';

      this.canvas.add(barcode);
      this.canvas.setActiveObject(barcode);
      this.canvas.requestRenderAll();

      // Auto-select to trigger properties panel
      this.onSelection({ selected: [barcode] });
    });
  }
  private addTriangle(x: number, y: number) {
    fabric.loadSVGFromString(Constants.SVGTemplate.Triangle, (objects, options) => {
      const triangle = fabric.util.groupSVGElements(objects, options);

      triangle.set({
        left: x,
        top: y,
        selectable: true,
        hasControls: true,
        hasBorders: true,
        strokeUniform: true
      });

      triangle.Id = this.generateId(WidgetType.Triangle);
      triangle.IsDynamic = false;
      triangle.CustomType = 'triangle';

      this.canvas.add(triangle);
      this.bringAllRequiredObjectsToFront();
      this.canvas.setActiveObject(triangle);
      this.canvas.requestRenderAll();

      // Auto-select for properties panel
      this.onSelection({ selected: [triangle] });
    });
  }
  private addLine(x: number, y: number) {
    fabric.loadSVGFromString(Constants.SVGTemplate.Line, (objects, options) => {
      const line = fabric.util.groupSVGElements(objects, options);
      line.set({
        left: x,
        top: y,
        selectable: true,
        hasControls: true,
        hasBorders: true
      });

      line.Id = this.generateId(WidgetType.Line);
      line.IsDynamic = false;
      line.CustomType = 'line';

        this.canvas.add(line);
        this.bringAllRequiredObjectsToFront();
        this.canvas.setActiveObject(line);
        this.canvas.requestRenderAll();
  
        // Auto-select on add
        this.onSelection({ selected: [line] });
      });
    }
  
    // Draw a circle at the drop position
    private addCircle(x: number, y: number) {
      
          fabric.loadSVGFromString(Constants.SVGTemplate.Circle, (objects, options) => {
          const circle = fabric.util.groupSVGElements(objects, options);
  
          circle.set({
            left: x,
            top: y,
            selectable: true,
            strokeUniform: true
          });
          circle.Id = this.generateId(WidgetType.Circle);
          circle.IsDynamic = false;
          circle.CustomType = 'circle';
  
          this.canvas.add(circle);
          this.bringAllRequiredObjectsToFront();
          this.canvas.setActiveObject(circle);
          this.canvas.renderAll();
  
          // ✅ Auto-select the new circle and trigger your properties panel logic
          this.onSelection({ selected: [circle] });
        });
    }
  
  saveCanvas(template: TemplateNew): Observable<any> {
    if (!this.isCanvasBlank()) {
      const payload = { items: this.canvas };
      console.log(payload);

      template.fabricJson = this.saveAsJSON();
      template.sysId = "00000000-0000-0000-0000-000000000000";
      template.canvasSettings = "{}";
      template.templateSvg = this.canvas.toSVG({

        height: 100,
        width: 200
      }).toString();
      template.versionId = this.templateVersionId;

      this.saveAsSVG();
      if (this.templateId.length > 0) {
        template.sysId = this.templateId;

        const url = Constants.ApiBaseUrl + Constants.SaveTemplateUrl + '/' + template.sysId;
        return this.http.put(url, template);
      } else {
        const url = Constants.ApiBaseUrl + Constants.SaveTemplateUrl;
        return this.http.post(url, template);
      }
    }
  }
  
       saveAsSVG(): string {
          console.log( this.canvas.toSVG());
          console.log(this.saveAsJSON());
          return  this.canvas.toSVG()
        }
  
        saveAsJSON(): string {

          let json = this.canvas.toJSON([
            'width',
            'height',
            'backgroundColor'
          ]);

    // Add custom counters for next IDs
    (json as any).idCounters = this.idCounters;
    return JSON.stringify(json);
  }
  isCanvasBlank(): boolean {
    return this.canvas.getObjects().length === 0;
  }
  deleteSelected() {
    const activeObject = this.canvas.getActiveObject();
    if (activeObject) {
      this.canvas.remove(activeObject);
      this.canvas.discardActiveObject();
      this.canvas.requestRenderAll();
    }
  }

  resetCanvas() {
    this.canvas.clear();
    // Optional: restore background color (since clear() wipes it too)
    this.canvas.setBackgroundColor('#f9fafb', this.canvas.renderAll.bind(this.canvas));

    // Clear selection & emit null
    this.itemSelected.emit(null);


  }

  getPreviewDataURL(): string {
    // Export as PNG Data URL (no selection, no borders)
    this.canvas.discardActiveObject();
    this.canvas.renderAll();

    return this.canvas.toDataURL({
      format: 'png',
      quality: 1,
      multiplier: 2, // higher resolution
      withoutTransform: true
    });

    //return this.canvas.toSVG();
  }

  private generateId(prefix: WidgetType): string {

    return WidgetType[prefix] + (++this.idCounters);
  }

  loadCanvasFromSVG(svg: string) {
    fabric.loadSVGFromString(svg, (objects, options) => {
      // Group all SVG elements together
      const objGroup = fabric.util.groupSVGElements(objects, options);

      // Optional: clear current canvas before loading new content
      this.canvas.clear();

      // Add the group or individual objects
      this.canvas.add(objGroup);

      // Set canvas object properties
      this.canvas.getObjects().forEach(obj => {
        obj.selectable = true;

        if (obj instanceof fabric.Textbox) {
          obj.editable = true; // Note: `editable` is only used when you enable editing
        }
      });

      this.canvas.renderAll();
    });
  }

  loadCanvasFromJSON(json: string) {
    this.canvas.loadFromJSON(json, () => {
      this.canvas.getObjects().forEach(obj => {
        obj.selectable = true;
        if (obj instanceof fabric.Textbox) {
          obj.editable = true;
        }
      });

      this.canvas.renderAll();
    });
  }

  //private emitPreview() {
  //  const dataUrl = this.canvas.toDataURL({
  //    format: 'png',
  //    // optional: you can set multiplier, background, etc.
  //  });
  //  this.canvasChanged.emit(dataUrl);
  //}

  private previewDebounceTimer: any;

  emitPreview() {
    clearTimeout(this.previewDebounceTimer);
    this.previewDebounceTimer = setTimeout(() => {
      const dataUrl = this.canvas.toDataURL({
        format: 'png',
        multiplier:0.5, // Optional: reduce image size for speed
      });
      this.canvasChanged.emit(dataUrl);
    }, 300); // Waits 300ms after last change
  }

  downloadPdf(name: string){

    // Convert Fabric canvas to PNG
    const dataURL = this.canvas.toDataURL({
      format: "png",
      quality: 1,
    });

    // Create PDF
    const pdf = new jsPDF("l", "pt", [this.canvas.getWidth(), this.canvas.getHeight()]);
    pdf.addImage(dataURL, "PNG", 0, 0, this.canvas.getWidth(), this.canvas.getHeight());

    // Save
    pdf.save(`${name}.pdf`);
  }

  getEligibleCoordinates(): coords {

    // The coordinate to check
    const point = new fabric.Point(20, 20);

    // Loop through objects to see if the point is inside any object
    const objectAtPoint = this.canvas.getObjects().find(obj => obj.containsPoint(point));

    if (objectAtPoint) {
      return new coords(100, 100);
    } else {
        return new coords(10, 10);
    }
  }
}

