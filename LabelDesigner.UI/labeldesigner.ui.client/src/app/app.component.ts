import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports:[LayoutComponent],
  templateUrl: './app.component.html'
})
export class AppComponent {}


//import { Component, ViewChild } from '@angular/core';
//import { ToolboxComponent } from './toolbox/toolbox.component';
//import { CanvasEditorComponent } from './canvas-editor/canvas-editor.component';
//import { PropertiesPanelComponent } from './properties-panel/properties-panel.component';
//import { CommonModule } from '@angular/common';

//interface CanvasItem {
//  id: number;
//  type: string;
//  label: string;
//  x: number;
//  y: number;
//  bgColor?: string;
//  color?: string;
//  fontSize?: number;
//}

//@Component({
//  selector: 'app-root',
//  standalone: true,
//  imports: [
//            CommonModule,
//            ToolboxComponent, 
//            CanvasEditorComponent, 
//            PropertiesPanelComponent],
//  templateUrl: './app.component.html',
//})
//export class AppComponent {

//  canvasItems: CanvasItem[] = [];
//  selectedItem: CanvasItem | null = null;

//  // Called when an item on the canvas is selected
//  onItemSelected(item: CanvasItem | null) {
//    this.selectedItem = item;
//  }

//  // Update canvasItems from CanvasEditorComponent
//  onCanvasItemsChange(items: CanvasItem[]) {
//    this.canvasItems = items;
//    // If selectedItem was deleted, reset
//    if (this.selectedItem && !items.find(i => i.id === this.selectedItem!.id)) {
//      this.selectedItem = null;
//    }
//  }

//  // Update properties of selected item
//  onItemUpdated(updatedItem: CanvasItem) {
//    const index = this.canvasItems.findIndex(i => i.id === updatedItem.id);
//    if (index !== -1) {
//      this.canvasItems[index] = { ...updatedItem }; // update with new reference
//    }
//  }
//}
