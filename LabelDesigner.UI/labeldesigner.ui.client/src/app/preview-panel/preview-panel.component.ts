import { Component, Input } from '@angular/core';
import { SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-preview-panel',
  standalone: true,
  templateUrl: './preview-panel.component.html',
  styleUrl: './preview-panel.component.css',
  imports: []
})
export class PreviewPanelComponent {

  @Input() previewSvg: SafeHtml;
}
