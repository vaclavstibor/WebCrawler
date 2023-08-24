import { Component } from '@angular/core';
import { Injectable } from '@angular/core';

export enum Mode {
  Website,
  Domain
}

@Component({
  selector: 'app-website-record',
  templateUrl: './website-record.component.html',
  styleUrls: ['./website-record.component.css']
})

@Injectable()
export class WebsiteRecordComponent {
  public mode: Mode = Mode.Website;
  public Mode = Mode;

  constructor() { }
}