import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';
import { ActivatedRoute } from '@angular/router';
import { WebsiteRecord } from '../models/WebsiteRecord';

@Component({
  selector: 'app-edit-record',
  templateUrl: './edit-record.component.html',
  styleUrls: ['./edit-record.component.css']
})
export class EditRecordComponent implements OnInit {
  record!: WebsiteRecord;

  constructor(private sharedService: SharedService,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
      if (id) {
        this.sharedService.getWebRecord(id).subscribe(data => {
          this.record = data;          
        });
      }
    
  }
}
