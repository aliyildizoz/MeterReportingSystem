import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MeterReading } from './models/meter';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MeterService {

  private baseUrl = "http://localhost:3515/api/meter";

  private url = (path: string) => `${this.baseUrl}/${path}`;
  constructor(private http: HttpClient) { }

  getAllMeter(): Observable<MeterReading[]> {
    return this.http.get<MeterReading[]>(this.baseUrl);
  }
  getMetersById(id: string): Observable<MeterReading> {
    return this.http.get<MeterReading>(this.url(id));
  }
  getMeterLastReading(serialNumber: string): Observable<MeterReading> {
    return this.http.get<MeterReading>(this.url(`GetLastReading/${serialNumber}`));
  }
  postMeter(meter: MeterReading): Observable<MeterReading> {
    return this.http.post<MeterReading>(this.baseUrl, meter);
  }

}
