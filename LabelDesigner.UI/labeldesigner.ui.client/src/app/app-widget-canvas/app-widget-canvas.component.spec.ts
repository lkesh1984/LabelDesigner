import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppWidgetCanvasComponent } from './app-widget-canvas.component';

describe('AppWidgetCanvasComponent', () => {
  let component: AppWidgetCanvasComponent;
  let fixture: ComponentFixture<AppWidgetCanvasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppWidgetCanvasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppWidgetCanvasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
