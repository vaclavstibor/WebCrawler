import { Component, Input } from '@angular/core';
import { Injectable } from '@angular/core';
import { WebsiteModeComponent } from './graph-mode/website-mode/website-mode.component';
import { DomainModeComponent } from './graph-mode/domain-mode/domain-mode.component';

export enum Mode {
  Website,
  Domain
}

export enum State {
  Live,
  Static
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

  public state: State = State.Live;
  public State = State;

  websiteModeComponent!: WebsiteModeComponent;
  domainModeComponent!: DomainModeComponent;
  
  constructor (
    ) {}

  onModeClick(): void {
    this.mode = this.mode === Mode.Website ? Mode.Domain : Mode.Website; // Because data-on-toggle gives only true/false via ngModel    
    console.log("onModeClick(): ", Mode[this.mode]);
  }

  onStateClick(): void {
    this.state = this.state === State.Live ? State.Static : State.Live; // Because data-on-toggle gives only true/false via ngModel
    console.log("onStateClick(): ", State[this.state]);
    
    switch (this.mode)
    {
      case Mode.Website:
      //this.websiteModeComponent.it;
      console.log("Mode: ", Mode[this.mode]);
      //this.websiteModeComponent.itWorks();
      break;
      case Mode.Domain:
      //console.log("Mode: ", Mode[this.mode]);
      //this.domainModeComponent.itWorks();
      break;
    }
  }
}