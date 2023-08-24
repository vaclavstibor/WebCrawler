import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WebsiteModeComponent } from './website-mode.component';

describe('WebsiteModeComponent', () => {
  let component: WebsiteModeComponent;
  let fixture: ComponentFixture<WebsiteModeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [WebsiteModeComponent]
    });
    fixture = TestBed.createComponent(WebsiteModeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
