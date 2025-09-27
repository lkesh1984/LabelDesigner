import { WidgetType } from "./widget-type.enum";

export interface Widget
{
  type: WidgetType;
  x: number;
  y: number;
  width: number;
  height: number;
  fillColor?: string;
  strokeColor?: string;
  strokeWidth?: number;
  color?: string;  // for barcode
}
