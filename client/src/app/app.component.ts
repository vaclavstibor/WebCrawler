import { Component, OnInit } from '@angular/core';
import { SharedService } from './shared.service';
import { ActivatedRoute } from '@angular/router';
import { WebsiteRecord } from './models/WebsiteRecord';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Hello';
  message: WebsiteRecord[] = []; 

  constructor(private sharedService: SharedService,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.sharedService.getWebRecords().subscribe(data => {
      this.message = data;
    });
  }
}