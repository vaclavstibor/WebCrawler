import { Component, OnInit } from '@angular/core';
import { SharedService } from './shared.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Hello';
  message: string = ""; 

  constructor(private sharedService: SharedService,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get("id"));
    this.sharedService.getMessage().subscribe(data => {
      this.message = data;
    });
  }
}