import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LayoutComponent } from './layout/layout.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TemplatesComponent } from './templates/templates.component';
import { DesignerModalComponent } from './designer-modal/designer-modal.component';
import { AppDesignerHeaderComponent } from './app-designer-header/app-designer-header.component';
import { AppWidgetCanvasComponent } from './app-widget-canvas/app-widget-canvas.component';
import { WidgetPropertyPanelComponent } from './widget-property-panel/widget-property-panel.component';

@NgModule({
  declarations: [
    AppComponent,
    LayoutComponent,
    DashboardComponent,
    TemplatesComponent,
    DesignerModalComponent,
    AppDesignerHeaderComponent,
    AppWidgetCanvasComponent,
    WidgetPropertyPanelComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule
  ],
  bootstrap: [AppComponent] // only bootstrap AppComponent
})
export class AppModule {}
