import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WidgetType } from '../shared/models/widget-type.enum';
import { FormsModule } from '@angular/forms';
import { FocusWhenDirective } from '../focus-when.directive';

@Component({
  selector: 'app-designer-header',
  standalone: true,
  imports: [CommonModule, FormsModule, FocusWhenDirective],
  templateUrl: './app-designer-header.component.html',
  styleUrls: ['./app-designer-header.component.css']
})
export class DesignerHeaderComponent {
  @Input() labelName: string = 'Template 1';
  @Output() labelSaved = new EventEmitter<string>();
  @Output() shapeSelected = new EventEmitter<WidgetType>();
  @Output() close = new EventEmitter<void>();

  isEditingName = false;
  selectedShape: WidgetType = WidgetType.None;
  WidgetType = WidgetType; // expose enum to template

  enableEditing() {
    this.isEditingName = true;
  }

  saveLabelName() {
    this.isEditingName = false;
    this.labelSaved.emit(this.labelName);
  }

  handleKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.saveLabelName();
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
  onDragStart(event: DragEvent, item: any) {
    event.dataTransfer?.setData('application/json', JSON.stringify(item));
  }
}
