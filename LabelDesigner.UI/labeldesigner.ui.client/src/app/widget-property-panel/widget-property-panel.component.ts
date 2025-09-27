import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Widget } from '../shared/models/widget.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-widget-property-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './widget-property-panel.component.html',
  styleUrls: ['./widget-property-panel.component.css']
})
export class WidgetPropertyPanelComponent {
  @Input() selectedWidget: Widget | null = null;
  @Output() widgetChanged = new EventEmitter<void>();

  onChange() {
    this.widgetChanged.emit();
  }
}
