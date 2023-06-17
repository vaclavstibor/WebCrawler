import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WebsiteRecordComponent } from './website-record.component';

describe('WebsiteRecordComponent', () => {
  let component: WebsiteRecordComponent;
  let fixture: ComponentFixture<WebsiteRecordComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [WebsiteRecordComponent]
    });
    fixture = TestBed.createComponent(WebsiteRecordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
