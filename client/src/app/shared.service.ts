import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  ApiUrl = "https://localhost:7170/api";

  constructor(private http: HttpClient) { }

  // getWebRecordDetail(id: number): Observable<IRecord> {
  //   return this.http.get<IRecord>(this.ApiUrl + "/Record/" + id);
  // }

  getMessage(): Observable<string> {
    return this.http
      .get(this.ApiUrl , {observe : "body", responseType: "text"});
  }
}
