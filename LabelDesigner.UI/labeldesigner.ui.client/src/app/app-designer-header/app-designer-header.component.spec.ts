import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppDesignerHeaderComponent } from './app-designer-header.component';

describe('AppDesignerHeaderComponent', () => {
  let component: AppDesignerHeaderComponent;
  let fixture: ComponentFixture<AppDesignerHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppDesignerHeaderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppDesignerHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
