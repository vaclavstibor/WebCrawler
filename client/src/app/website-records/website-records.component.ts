import { Component, OnInit } from '@angular/core';
import { WebsiteRecord } from '../models/WebsiteRecord';
import { SharedService } from '../shared.service';
import { ActivatedRoute } from '@angular/router';
import { Tag } from '../models/Tag';

@Component({
  selector: 'app-website-records',
  templateUrl: './website-records.component.html',
  styleUrls: ['./website-records.component.css']
})
export class WebsiteRecordsComponent implements OnInit {
  webRecords: WebsiteRecord[] = [];
  tags: string[] = [];

  newRecordBeingCreated: boolean = false;
  newRecord: WebsiteRecord = <WebsiteRecord>{};
  newTag: Tag = <Tag>{};  

  constructor(private sharedService: SharedService,
  private route: ActivatedRoute) { }

    private _chosenTags: string[] = [];
    get chosenTags(): string[] {
      return this._chosenTags;
    }
    set chosenTags(value: string[]) {
      this._chosenTags = value;
    }
    

    private _chosenUrls: string[] = [];
    get chosenUrls(): string[] {
      return this._chosenUrls;
    }
    set chosenUrls(value: string[]) {
      this._chosenUrls = value;
    }


    private _chosenLabels: string[] = [];
    get chosenLabels(): string[] {
      return this._chosenLabels;
    }
    set chosenLabels(value: string[]) {
      this._chosenLabels = value;
    }

  ngOnInit(): void {
    this.sharedService.getWebRecords().subscribe(data => {
      this.webRecords = data;
      this.webRecords.forEach(webRecord => {
        webRecord.tagDTOs.forEach(tag => {
          this.tags.push(tag.content);
        });
      });
    });
  }
}