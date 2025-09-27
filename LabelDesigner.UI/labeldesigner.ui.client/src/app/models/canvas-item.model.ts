export interface CanvasItem {
  id: string;
  type: string;
  label?: string;
  bgColor?: string;
  color?: string;
  fontSize?: number;
  bold?: boolean;
  italic?: boolean;
  underline?: boolean;
  isDynamic?: boolean
  src?: string;           // for images
  fabricObject?: any;     // reference to the Fabric object
  height?: number;
  width?: number;
  barcodeType?: string;
  linethrough?: boolean,
  stroke?: string;  
  strokeWidth?: string; 
  left?: number,
  top?: number
}
