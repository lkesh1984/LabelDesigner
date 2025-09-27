import { Widget } from "./widget.model";

export class Template {
  sysId: string;
  name: string;
  widgets: any[];
  templateSvg: string;
  fabricJson: string;

  constructor(data?: Partial<Template>) {
    this.sysId = data?.sysId || '';
    this.name = data?.name || 'Label Template';
    this.widgets = data?.widgets || [];
    this.templateSvg = data?.templateSvg;
    this.fabricJson = data?.fabricJson
  }

  addWidget(widget: any) {
    this.widgets.push(widget);
  }
}
