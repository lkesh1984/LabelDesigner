export class CanvasSettings
{
  viewWidth: number = 200;
  viewHeight: number = 100;

  get viewBox(): string
  {
    return `0 0 ${this.viewWidth} ${this.viewHeight}`;
  }
}
