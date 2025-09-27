import { bootstrapApplication } from '@angular/platform-browser';
import { LayoutComponent } from './app/layout/layout.component';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { appRoutes } from './app/app.routes';

bootstrapApplication(LayoutComponent, {
  providers: [
    provideRouter(appRoutes),
    provideAnimations()
  ]
});
