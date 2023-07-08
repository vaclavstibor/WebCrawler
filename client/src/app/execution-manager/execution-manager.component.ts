import { Component, OnInit } from '@angular/core';
import { WebsiteRecord } from '../models/WebsiteRecord';
import { SharedService } from '../shared.service';
import { ActivatedRoute } from '@angular/router';
import { Tag } from '../models/Tag';
import { UpdateRecordState } from '../models/UpdateRecordState';
import { Router } from '@angular/router';
import { Execution } from '../models/Execution';

@Component({
  selector: 'app-website-records',
  templateUrl: './execution-manager.component.html',
  styleUrls: ['./execution-manager.component.css']
})
export class ExecutionManagerComponent implements OnInit {
  allExecutions: Execution[] = [];
  executions: Execution[] = [];

  sortByLastCrawling: boolean = false;

  constructor(private sharedService: SharedService,
  private route: ActivatedRoute,
  private router: Router) { }

  private _chosenLabels: string[] = [];
  get chosenLabels(): string[] {
    return this._chosenLabels;
  }
  set chosenLabels(value: string[]) {
    this._chosenLabels = value;
  }

  ngOnInit(): void {
    this.executions = [];
    this.allExecutions = [];

    this.sharedService.getExecutions().subscribe(data => {
      this.executions = data;
      this.allExecutions = data;
    });
  }

  index: number = 0;

  fromNewest: boolean = false;

  filter() : void
  {
    this.executions = this.allExecutions;
    if (this.chosenLabels.length > 0)
    {
      this.executions = this.executions.filter(x => this.chosenLabels.includes(x.websiteRecordLabel));
    }
  }

  executeSortByLastCrawling(fromNewest: boolean) : void
  {
    this.fromNewest = fromNewest;
    if (this.sortByLastCrawling)
    {
      if (fromNewest)
      {
        this.executions.sort((a,b) => { 
          if (a.startTime == null || b.startTime == null)
          {
            return 0;
          }
          return a.startTime.getTime() - b.startTime.getTime(); 
        });
      }
      else 
      {
        this.executions.sort((b,a) => { 
          if (a.endTime == null || b.endTime == null)
          {
            return 0;
          }
          return a.endTime.getTime() - b.endTime.getTime(); 
        });
      }
    }
  }

  columns: any[] = 
  [
    {
      name: "Website Record Label",
      field: "websiteRecordLabel"
    },
    {
      name: "Execution Status",
      field: "executionStatus"
    },
    {
      name: "Number Of Sites Crawled",
      field: "numberOfSitesCrawled"
    },
    {
      name: "Start Time",
      field: "startTime"
    },
    {
      name: "End Time",
      field: "endTime",
    }
  ];

  navigateToView(id: number) : void
  {
    this.router.navigate(["record/view" , id]);
  }
}