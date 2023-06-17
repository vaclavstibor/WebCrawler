import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WebsiteRecord } from './models/WebsiteRecord';
import { Tag } from './models/Tag';

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

  getMessage(): Observable<string> {
    return this.http
      .get(this.ApiUrl , {observe : "body", responseType: "text"});
  }
}
