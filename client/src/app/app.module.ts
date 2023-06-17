import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from "@angular/common/http"

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { WebsiteRecordsComponent } from './website-records/website-records.component';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { WebsiteRecordComponent } from './website-record/website-record.component'

@NgModule({
  declarations: [
    AppComponent,
    WebsiteRecordsComponent,
    WebsiteRecordComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
