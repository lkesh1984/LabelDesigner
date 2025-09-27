export class TemplateNew {
  sysId: string;
  versionId: string;
  name: string;
  fabricJson: string;
  canvasSettings: string;
  templateSvg: string;

  constructor(data?: Partial<TemplateNew>) {
    this.sysId = data?.sysId || '';
    this.versionId = data?.versionId || '';
    this.name = data?.name || 'Label Template';
    this.fabricJson = data?.fabricJson;
    this.templateSvg = data?.templateSvg;
    this.versionId = data?.versionId
  }
}
