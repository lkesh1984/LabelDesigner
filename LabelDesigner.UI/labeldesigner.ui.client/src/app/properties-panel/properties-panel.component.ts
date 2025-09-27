import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { fabric } from 'fabric';
import { CanvasItem } from '../models/canvas-item.model';

@Component({
  selector: 'app-properties-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './properties-panel.component.html',
  styleUrls: ['./properties-panel.component.css']
})
export class PropertiesPanelComponent {
  @Input() item!: CanvasItem;
  @Output() itemUpdated = new EventEmitter<CanvasItem>();
  private _previousTextValue: string;
  barcodeTypes: string[] = [ 'Code128', 'QRCode' ];

  // Triggered on any property change
  itemChange() {
    // Update Fabric object in real-time
    if (this.item.fabricObject) {
      
      this.item.fabricObject.set({

        left: this.item.left && this.item.left > 0 ? this.item.left : this.item.fabricObject.top,
        top: this.item.top && this.item.top > 0 ? this.item.top : this.item.fabricObject.top,
        
      });
      switch (this.item.type) {
        case 'textbox':
          (this.item.fabricObject as fabric.Textbox).set({
            text: this.item.label,
            fill: this.item.color,
            fontSize: this.item.fontSize,
            fontWeight: this.item.bold ? 'bold' : 'normal',
            fontStyle: this.item.italic ? 'italic' : 'normal',
            underline: this.item.underline || false,
            editable: !this.item.isDynamic,
            linethrough: this.item.linethrough,
            //backgroundColor: this.item.isDynamic ? '#f0f0f0' : 'transparent'
          });
          this.item.fabricObject.IsDynamicnamic = this.item.isDynamic;

          break;

        case 'image':
          if (this.item.src) {
            fabric.Image.fromURL(this.item.src, (newImg) => {
              this.item.fabricObject.setElement(newImg.getElement());
              this.item.fabricObject.set({
                left: this.item.left,
                top: this.item.top,
                scaleX: this.item.fabricObject.scaleX,
                scaleY: this.item.fabricObject.scaleY,
              });
              this.item.fabricObject.canvas.requestRenderAll();
            });
          }
          break;
        case 'line':
          this.item.fabricObject.set({
            stroke: this.item.bgColor,
            strokeWidth: this.item.strokeWidth
          });
          break;
        case 'barcode':
          this.item.fabricObject.BarcodeType = this.item.barcodeType;
          let src = '';
          if(this.item.barcodeType === 'QRCode') {
            src = '/images/QRCode.png';
          }
          else{
              src = '/images/barcode.png';
          }
          this.item.fabricObject.setSrc(src, ()=>{

          });
          break;
        case "canvas":
            // do nothing
          break;

        default: // shapes like circle, triangle, line
          this.item.fabricObject.set({
            fill: this.item.bgColor,
            stroke: this.item.stroke,
            strokeWidth: this.item.strokeWidth
          });
      }
      this.item.fabricObject.canvas.requestRenderAll();
    }

    this.itemUpdated.emit(this.item); // emit updated item
  }

  toggleStyle(style: 'bold' | 'italic' | 'underline' | 'linethrough') {
    this.item[style] = !this.item[style];
    this.itemChange();
  }

  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const reader = new FileReader();
      reader.onload = () => {
        if (this.item) {
          this.item.src = reader.result as string;
          this.itemChange();
        }
      };
      reader.readAsDataURL(input.files[0]);
    }
  }

  onCheckChange()
  {
    if(this.item.isDynamic)
    {
      this._previousTextValue = this.item.label;
      this.item.label = '{{Text}}'
    }
    else if(this._previousTextValue && this._previousTextValue !==  '')
    {
      this.item.label = this._previousTextValue;
    }

    this.itemChange();
  }
}
