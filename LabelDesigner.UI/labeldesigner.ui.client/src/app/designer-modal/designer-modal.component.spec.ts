import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DesignerModalComponent } from './designer-modal.component';

describe('DesignerModalComponent', () => {
  let component: DesignerModalComponent;
  let fixture: ComponentFixture<DesignerModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DesignerModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DesignerModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
