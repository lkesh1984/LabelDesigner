import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WidgetType } from '../shared/models/widget-type.enum';
import { FormsModule } from '@angular/forms';
import { FocusWhenDirective } from '../focus-when.directive';
import { ToolboxItem } from '../models/ToolboxItem.model';

@Component({
  selector: 'app-toolbox',
  standalone: true,
  templateUrl: './toolbox.component.html',
  styleUrls: ['./toolbox.component.css'],
  imports: [
            CommonModule, 
            FormsModule, 
            FocusWhenDirective
          ],
})
export class ToolboxComponent { 
  @Input() templateName: string = 'Label Template';
  @Output() updateTemplateNameEvent = new EventEmitter<string>();
  @Output() shapeSelected = new EventEmitter<WidgetType>();
  @Output() close = new EventEmitter<void>();
  @Output() onShapeDoubleClick = new EventEmitter<ToolboxItem>();

  isEditingName = false;
  selectedShape: WidgetType = WidgetType.None;
  WidgetType = WidgetType; // expose enum to template
  items: ToolboxItem[] = [
    { type: WidgetType.Text, label: 'Text', iconClass: 'fas fa-font' },
    { type: WidgetType.Line, label: 'Line', iconClass: 'fas fa-minus' },
    { type: WidgetType.Rectangle, label: 'Rectangle', iconClass: 'fas fa-minus' },
    { type: WidgetType.Triangle, label: 'Triangle', iconClass: 'fas fa-minus' },
    { type: WidgetType.Barcode, label: 'Barcode', iconClass: 'fas fa-barcode' },
    { type: WidgetType.Image, label: 'Image', iconClass: 'fas fa-image' },
     { type: WidgetType.Circle, label: 'Circle', iconClass: 'fas fa-circle' }  
  ];

  customTemplates: ToolboxItem[] = [
    { type: WidgetType.Custom, label: 'Custom1', iconClass: 'fas fa-font' } 
  ];

  enableEditing() {
    this.isEditingName = true;
  }

  saveTemplateName() {
    this.isEditingName = false;
    this.updateTemplateNameEvent.emit(this.templateName);
  }

  handleKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.saveTemplateName();
    } else if (event.key === 'Escape') {
      this.isEditingName = false;
    }
  }

  selectShape(shape: WidgetType) {
    this.selectedShape = shape;
    this.shapeSelected.emit(shape);
  }

  closeDesigner() {
    this.close.emit();
  }

  // Called on drag start to pass data to canvas
  onDragStart(event: DragEvent, item: ToolboxItem) {
    event.dataTransfer?.setData('application/json', JSON.stringify(item));
  }

  ondblClick(event: any, item: ToolboxItem){

      this.onShapeDoubleClick.emit(item);
  }
}
