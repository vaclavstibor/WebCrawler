import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-website-record',
  templateUrl: './website-record.component.html',
  styleUrls: ['./website-record.component.css']
})
export class WebsiteRecordComponent {
  constructor(private route: ActivatedRoute)
  {
    
  }
}
