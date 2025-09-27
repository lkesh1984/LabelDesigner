import { WidgetType } from "../shared/models/widget-type.enum";

export interface ToolboxItem {
  type: WidgetType;
  label: string;
  iconClass: String;
}