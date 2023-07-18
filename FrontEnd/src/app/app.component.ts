import { Component } from '@angular/core';
import { SharedService } from './shared.service';
import { ActivatedRoute } from '@angular/router';
import { WebsiteRecord } from './models/WebsiteRecord';
import { MenuItem } from 'primeng/api/menuitem';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent{
  title = 'Hello';
  message: WebsiteRecord[] = []; 
  items: MenuItem[] = [
    { label: "Website Records", routerLink:["records"] },
    { label: "Execution Manager", routerLink:["executionManager"] }
  ];

  constructor(private sharedService: SharedService,
    private route: ActivatedRoute,
    private router: Router) { }
}