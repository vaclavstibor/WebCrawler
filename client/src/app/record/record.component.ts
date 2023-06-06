import { Component, OnInit } from '@angular/core';
import { IRecord } from './record';
import { SharedService } from '../shared.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-record',
  templateUrl: './record.component.html',
  styleUrls: ['./record.component.css']
})
export class RecordComponent implements OnInit {
  record: IRecord | undefined;
  pageTitle: string = "Record detail";

  constructor(private sharedService: SharedService,
              private route: ActivatedRoute) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get("id"));
    this.sharedService.getWebRecordDetail(id).subscribe(data => {
      this.record = data;
    });
  }
}