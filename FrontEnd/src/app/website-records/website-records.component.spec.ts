import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WebsiteRecordsComponent } from './website-records.component';

describe('WebsiteRecordsComponent', () => {
  let component: WebsiteRecordsComponent;
  let fixture: ComponentFixture<WebsiteRecordsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [WebsiteRecordsComponent]
    });
    fixture = TestBed.createComponent(WebsiteRecordsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
