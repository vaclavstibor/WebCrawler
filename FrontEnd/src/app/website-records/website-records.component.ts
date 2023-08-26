import { Component, OnInit } from '@angular/core';
import { WebsiteRecord } from '../models/WebsiteRecord';
import { SharedService } from '../shared.service';
import { ActivatedRoute } from '@angular/router';
import { Tag } from '../models/Tag';
import { UpdateRecordState } from '../models/UpdateRecordState';
import { Router } from '@angular/router';

@Component({
  selector: 'app-website-records',
  templateUrl: './website-records.component.html',
  styleUrls: ['./website-records.component.css']
})
export class WebsiteRecordsComponent implements OnInit {
  allWebRecords: WebsiteRecord[] = [];
  webRecords: WebsiteRecord[] = [];
  tags: string[] = [];

  updateRecordText: string = "Create New Record";

  sortByUrl: boolean = false;
  sortByLastCrawling: boolean = false;

  recordUpdatingState: UpdateRecordState = UpdateRecordState.default;
  defaultState: UpdateRecordState = UpdateRecordState.default;

  newRecord: WebsiteRecord = <WebsiteRecord>{};
  newTag: Tag = <Tag>{};  

  constructor(private sharedService: SharedService,
  private route: ActivatedRoute,
  private router: Router) { }

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

  createNewRecord() : void 
  {
    if (this.recordUpdatingState == UpdateRecordState.default || this.recordUpdatingState == UpdateRecordState.updating)
    {
      this.recordUpdatingState = UpdateRecordState.creating;
    }
    else 
    {
      this.recordUpdatingState = UpdateRecordState.default;
    }
    this.updateRecordText = "Add record";
    this.newRecord = <WebsiteRecord>{};
  }

  addTag() : void {
    this.newRecord.tagDTOs.push(this.newTag);
    this.newTag = <Tag>{};
  }

  updateRecord() : void {
    this.sharedService.updateRecord(this.newRecord).subscribe(data => 
      {
        this.newRecord.id = data;
      });

    var record: number = this.allWebRecords.findIndex(x => x.id == this.newRecord.id);  
    
    if (record === -1)
    {
      this.allWebRecords.push(this.newRecord);
    }
    else 
    {
      this.allWebRecords[record] = this.newRecord;
    }
    
    this.newRecord = <WebsiteRecord>{};
    this.newRecord.tagDTOs = [];
  }

  deleteARecord(record: WebsiteRecord) : void
  {
    this.sharedService.deleteRecord(record).subscribe();
    this.webRecords = this.webRecords.filter(x => x != record);
    this.allWebRecords = this.allWebRecords.filter(x => x != record);
  }

  ngOnInit(): void {
    this.newRecord.tagDTOs = [];
    this.webRecords = [];
    this.allWebRecords = [];

    this.sharedService.getWebRecords().subscribe(data => {
      this.webRecords = data;
      this.allWebRecords = data;
      this.webRecords.forEach(webRecord => {
        webRecord.tagDTOs.forEach(tag => {
          this.tags.push(tag.content);
        });
      });
    });
  }

  index: number = 0;

  removeTagFromNewRecord(tag: Tag) : void
  {
    this.index = this.newRecord.tagDTOs.indexOf(tag);
    this.newRecord.tagDTOs.splice(this.index,1);
  }

  alphabetically: boolean = false;
  fromNewest: boolean = false;

  executeSortByUrl(alphabetically: boolean) : void
  {
    this.alphabetically = alphabetically;
    if (this.sortByUrl)
    {
      if (alphabetically)
      {
        this.webRecords.sort((a,b) => a.url.localeCompare(b.url));
      }
      else 
      {
        this.webRecords.sort((a,b) => b.url.localeCompare(a.url));
      }
    }
  }

  filter() : void
  {
    this.webRecords = this.allWebRecords;
    if (this.chosenUrls.length > 0)
    {
      this.webRecords = this.webRecords.filter(x => this.chosenUrls.includes(x.url));
    }
    if (this.chosenLabels.length > 0)
    {
      this.webRecords = this.webRecords.filter(x => this.chosenLabels.includes(x.label));
    }
    if (this.chosenTags.length > 0)
    {
      this.webRecords = this.webRecords.filter(x => 
      {
        var shouldBeContained = false;
        x.tagDTOs.forEach(element => {
          if (this.chosenTags.includes(element.content))
          {
            shouldBeContained = true;
          }
        });  
        return shouldBeContained;
      });
    }
  }

  executeSortByLastCrawling(fromNewest: boolean) : void
  {
    this.fromNewest = fromNewest;
    if (this.sortByLastCrawling)
    {
      if (fromNewest)
      {
        this.webRecords.sort((b,a) => { 
          if (a.lastExecution == null || b.lastExecution == null)
          {
            return 0;
          }
          return a.lastExecution.getTime() - b.lastExecution.getTime(); 
        });
      }
      else 
      {
        this.webRecords.sort((a,b) => { 
          if (a.lastExecution == null || b.lastExecution == null)
          {
            return 0;
          }
          return a.lastExecution.getTime() - b.lastExecution.getTime(); 
        });
      }
    }
  }

  columns: any[] = 
  [
    {
      name: "URL",
      field: "url",
      style: "width: 15%; word-break: break-all;"
    },
    {
      name: "Label",
      field: "label",
      style: "width: 5%; word-break: break-all;"
    },
    {
      name: "Last Execution",
      field: "lastExecution",
      style: "width: 10%; word-break: break-all;"
    },
    {
      name: "Status of last Execution",
      field: "executionStatus",
      style: "width: 5%; word-break: break-all;"
    }
  ];

  navigateToView(id: number) : void
  {
    this.router.navigate(["record/view" , id]);
  }

  execute(record: WebsiteRecord) : void
  {
    this.sharedService.executeRecord(record.id).subscribe();
  }

  editRecord(id: number) : void
  {
    if (this.recordUpdatingState == UpdateRecordState.default || this.recordUpdatingState == UpdateRecordState.creating)
    {
      this.updateRecordText = "Update record";
      this.recordUpdatingState = UpdateRecordState.updating;
    }
    else if (this.newRecord.id == id)
    {
      this.recordUpdatingState = UpdateRecordState.default;
      this.updateRecordText = "Add record";
    }
    this.webRecords.forEach(record => {
      if (record.id == id)
      {
        this.newRecord = JSON.parse(JSON.stringify(record));
      }
    });
  }
}