// TODO Add static/live

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WebsiteRecord } from './models/WebsiteRecord';
import { Tag } from './models/Tag';
import { HttpHeaders } from '@angular/common/http';
import { Execution } from './models/Execution';
import { map } from 'rxjs/operators';
import { Node } from './models/Node';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  ApiUrl = "http://localhost:4200/api";

  constructor(private http: HttpClient) { }

  getGraph(id: number): Observable<Node[]> {
    return this.http.get<Node[]>(this.ApiUrl + "/Crawler/getGraph/" + id);
  }

  getGraphLive(id: number): Observable<Node[]> {
    return this.http.get<Node[]>(this.ApiUrl + "/Crawler/getGraph/live/" + id);
  }

  getGraphLiveInitial(id: number): Observable<Node[]> {
    return this.http.get<Node[]>(this.ApiUrl + "/Crawler/getGraph/live/all/" + id);
  }

  getWebRecord(id: number): Observable<WebsiteRecord> {
    return this.http.get<WebsiteRecord>(this.ApiUrl + "/Record/" + id);
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

  executeRecord(recordId: number) : Observable<any>
  {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<number>(this.ApiUrl + "/Crawler/execute", recordId, httpOptions);
  }

  getExecutionStatus(recordId: number): Observable<string> {
    return this.http.get<Execution[]>(`${this.ApiUrl}/Crawler/all`).pipe(
      map((executions: Execution[]) => {
        const matchingExecution = executions.find(execution => execution.websiteRecordId == recordId);
        return matchingExecution ? matchingExecution.executionStatus : '';
      })
    );
  }
}
