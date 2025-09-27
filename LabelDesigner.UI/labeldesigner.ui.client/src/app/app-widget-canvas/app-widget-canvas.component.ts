import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CanvasSettings } from '../shared/models/canvas-settings.model';
import { Widget } from '../shared/models/widget.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-widget-canvas',
  templateUrl: './app-widget-canvas.component.html',
  standalone: true,
  imports: [CommonModule],
  styleUrls: ['./app-widget-canvas.component.css']
})
export class WidgetCanvasComponent
{
  @Input() widgets: Widget[] = [];
  @Input() selectedWidget?: Widget;
  @Input() canvasSettings!: CanvasSettings;

  @Output() widgetSelected = new EventEmitter<Widget>();

  select(w: Widget)
  {
    this.widgetSelected.emit(w);
  }
}
