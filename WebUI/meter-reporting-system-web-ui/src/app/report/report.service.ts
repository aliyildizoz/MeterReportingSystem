import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReportRequest } from './models/report';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  private baseUrl = "http://localhost:3516/api/report";

  private url = (path: string) => `${this.baseUrl}/${path}`;
  constructor(private http: HttpClient) { }

  getAllReport(): Observable<ReportRequest[]> {
    return this.http.get<ReportRequest[]>(this.baseUrl);
  }
  getReportById(id: string): Observable<ReportRequest> {
    return this.http.get<ReportRequest>(this.url(id));
  }
  downloadReport(id: string): Observable<Blob> {
    return this.http.get(this.url(`Download/${id}`), { responseType: 'blob' });
  }

  createReportRequest(serialNumber: string): Observable<ReportRequest> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<ReportRequest>(this.baseUrl, JSON.stringify(serialNumber), { headers });
  }
}
