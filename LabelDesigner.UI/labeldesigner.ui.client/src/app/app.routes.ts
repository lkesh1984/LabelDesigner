import { Routes } from "@angular/router";
import { TemplatesComponent } from "./templates/templates.component";

export const appRoutes: Routes = [
  { path: '', component: TemplatesComponent },          // default path
  { path: 'templates', component: TemplatesComponent }, // optional path
  // add other routes here
];
