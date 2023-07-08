import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExecutionManagerComponent } from './execution-manager.component';

describe('WebsiteRecordsComponent', () => {
  let component: ExecutionManagerComponent;
  let fixture: ComponentFixture<ExecutionManagerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ExecutionManagerComponent]
    });
    fixture = TestBed.createComponent(ExecutionManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
