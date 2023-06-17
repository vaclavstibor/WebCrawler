import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from "@angular/common/http";
import { MultiSelectModule } from "primeng/multiselect";
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { WebsiteRecordsComponent } from './website-records/website-records.component';
import { WebsiteRecordComponent } from './website-record/website-record.component';

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
    MultiSelectModule,
    FormsModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
