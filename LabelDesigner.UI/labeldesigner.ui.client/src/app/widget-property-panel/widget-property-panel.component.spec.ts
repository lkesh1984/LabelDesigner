import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetPropertyPanelComponent } from './widget-property-panel.component';

describe('WidgetPropertyPanelComponent', () => {
  let component: WidgetPropertyPanelComponent;
  let fixture: ComponentFixture<WidgetPropertyPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WidgetPropertyPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WidgetPropertyPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
