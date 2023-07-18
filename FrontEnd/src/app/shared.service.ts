import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WebsiteRecord } from './models/WebsiteRecord';
import { Tag } from './models/Tag';
import { HttpHeaders } from '@angular/common/http';
import { Execution } from './models/Execution';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  ApiUrl = "https://localhost:44352/api";

  constructor(private http: HttpClient) { }

  getWebRecord(id: number): Observable<WebsiteRecord> {
    return this.http.get<WebsiteRecord>(this.ApiUrl + "/Record/" + id).pipe(
      map((record => {
            record.lastExecution = new Date(record.lastExecution);
            return record;
          }
        )
      )
    );
  }

  getTag(id: number): Observable<Tag> {
    return this.http.get<Tag>(this.ApiUrl + "/Tag" + id);
  }

  getWebRecords(): Observable<WebsiteRecord[]> {
    return this.http.get<WebsiteRecord[]>(this.ApiUrl + "/Record/all").pipe(
      map((records: WebsiteRecord[]) => 
        records.map(record => 
          {    
            record.lastExecution = new Date(record.lastExecution);
            return record;
          }
        ))
      );
  }

  getExecutions(): Observable<Execution[]> {
    return this.http.get<Execution[]>(this.ApiUrl + "/Crawler/all").pipe(
      map((executions: Execution[]) => 
        executions.map(
          execution => 
          {
            execution.startTime = new Date(execution.startTime);
            execution.endTime = new Date(execution.endTime);
            return execution;
          }
        )
      )
    )
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