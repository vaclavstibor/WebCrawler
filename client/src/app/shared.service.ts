import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IRecord } from './record/record';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  ApiUrl = "/api";

  constructor(private http: HttpClient) { }

  getWebRecordDetail(id: number): Observable<IRecord> {
    return this.http.get<IRecord>(this.ApiUrl + "/Record/" + id);
  }

  getMessage(): Observable<string> {
    return this.http.get<string>(this.ApiUrl + "Hello");
  }
}
