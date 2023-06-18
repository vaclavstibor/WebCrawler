import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WebsiteRecord } from './models/WebsiteRecord';
import { Tag } from './models/Tag';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  ApiUrl = "https://localhost:44352/api";

  constructor(private http: HttpClient) { }

  getWebRecord(id: number): Observable<WebsiteRecord> {
    return this.http.get<WebsiteRecord>(this.ApiUrl + "/Record/" + id);
  }

  getTag(id: number): Observable<Tag> {
    return this.http.get<Tag>(this.ApiUrl + "/Tag" + id);
  }

  getWebRecords(): Observable<WebsiteRecord[]> {
    return this.http.get<WebsiteRecord[]>(this.ApiUrl + "/Record/all");
  }

  updateRecord(record: WebsiteRecord) : Observable<any> 
  {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<WebsiteRecord>(this.ApiUrl + "/Record/updateSingle", record, httpOptions);
  }  

  deleteRecord(record: WebsiteRecord) : Observable<any>
  {
    return this.http.delete(this.ApiUrl + "/Record/deleteSingle/" + record.id);
  }
}
