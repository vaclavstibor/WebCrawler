import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DomainModeComponent } from './domain-mode.component';

describe('DomainModeComponent', () => {
  let component: DomainModeComponent;
  let fixture: ComponentFixture<DomainModeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DomainModeComponent]
    });
    fixture = TestBed.createComponent(DomainModeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
