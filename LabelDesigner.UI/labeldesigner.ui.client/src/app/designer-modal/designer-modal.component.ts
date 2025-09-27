import { Component, EventEmitter, input, Input, Output, ViewChild } from '@angular/core';
import { CanvasSettings } from '../shared/models/canvas-settings.model';
import { Widget } from '../shared/models/widget.model';
import { CommonModule } from '@angular/common';
import { ToolboxComponent } from '../toolbox/toolbox.component';
import { CanvasEditorComponent } from '../canvas-editor/canvas-editor.component';
import { PropertiesPanelComponent } from '../properties-panel/properties-panel.component';
import { CanvasItem } from '../models/canvas-item.model';
import { Template } from '../shared/models/template.model';
import { TemplateNew } from '../shared/models/templatenew.modal';
import { DesignerHeaderComponent } from '../app-designer-header/app-designer-header.component';
import { ChangeDetectorRef } from '@angular/core';
import { ToolboxItem } from '../models/ToolboxItem.model';
import { coords } from '../models/point.model';

@Component({
  selector: 'app-designer-modal',
  standalone: true,
  templateUrl: './designer-modal.component.html',
  styleUrls: ['./designer-modal.component.css'],
  imports: [
            CommonModule,
            ToolboxComponent,
            CanvasEditorComponent,
            PropertiesPanelComponent,
            DesignerHeaderComponent
          ] 
})
export class DesignerModalComponent {
  @Input() isOpen = false;
  @Input() templateId = '';
  @Input() templateSvg = '';
  @Input() fabricJson = '';
  @Input() templateVersionId = '';
  @Input() templateName = '';
  @ViewChild('designer') designer!: CanvasEditorComponent;

  canvasItems: CanvasItem[] = [];
  selectedItem: CanvasItem | null = null;
  previewUrl: string | null = null;

  @Input() template: TemplateNew = new TemplateNew();
  @Output() templateSavedEvent = new EventEmitter<TemplateNew>();
  @Output() closeDesignerEvent = new EventEmitter<void>();
  
  showProperties = true;

  widgets: Widget[] = [];
  canvasSettings: CanvasSettings = {
      viewWidth: 0,
      viewHeight: 0,
      viewBox: ''
  };
  selectedWidget: Widget = null;
  canvasPreviewUrl: string | null = null;
  constructor(private cdr: ChangeDetectorRef) { }
  updatePreview(imageDataUrl: string) {
    this.canvasPreviewUrl = imageDataUrl;
    this.cdr.detectChanges(); // optional but ensures view updates
  }
  updateTemplateName(templateName: string){
    this.template.name = templateName;
  }
  
  openDesigner() {
    this.isOpen = true;
  }

  editDesigner() {
    this.isOpen = true;

  }

  closeDesigner() {
    this.isOpen = false;
    this.closeDesignerEvent.emit();
  }

  toggleProperties() {
    this.showProperties = !this.showProperties;
  }

  handleWidgetSelected(widget: any) {
    this.selectedWidget = widget;
  }

  onWidgetChanged() {
    // refresh UI or trigger change detection if needed
  }

  saveTemplate() {
    if (!this.designer.isCanvasBlank()) {
      // save logic
      this.designer.saveCanvas(this.template).subscribe({
        next: (response) => {
          this.designer.saveAsSVG();
          // Emit event only after successful save
          this.templateSavedEvent.emit(response);
          this.closeDesigner();
        },
        error: (err) => {
          console.error('Save failed', err);
          // Optionally show error UI here
        }
      });
    }
  }

  
  onItemUpdated(updatedItem: CanvasItem) {

    this.designer.updateItem(updatedItem); 
  }

  // Called when an item on the canvas is selected
  onItemSelected(item: CanvasItem | null) {
    this.selectedItem = item;
  }

  // Update canvasItems from CanvasEditorComponent
  onCanvasItemsChange(items: CanvasItem[]) {
    this.canvasItems = items;
    // If selectedItem was deleted, reset
    if (this.selectedItem && !items.find(i => i.id === this.selectedItem!.id)) {
      this.selectedItem = null;
    }
  }

  resetDesigner() {
    if (this.designer) {
      this.designer.resetCanvas();
      this.canvasItems = [];
      this.selectedItem = null;
    }
  }

  previewDesigner() {
    if (this.designer) {
      this.previewUrl = this.designer.getPreviewDataURL();
    }
  }

  download(){

      this.designer.downloadPdf(this.templateName);
  }

  onShapedblClick(item: ToolboxItem){
      let point: coords = this.designer.getEligibleCoordinates();

      this.designer.drawShape(item, point.x, point.y);
  }
}
