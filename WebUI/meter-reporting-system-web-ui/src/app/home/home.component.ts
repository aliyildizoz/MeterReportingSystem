import { Component } from '@angular/core';
import { MeterService } from '../meter/meter.service';
import { FormsModule } from '@angular/forms';
import { MeterReading } from '../meter/models/meter';
import { CommonModule } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import { catchError, of } from 'rxjs';

export type alert = 'warning' | 'error' | 'info';
@Component({
  selector: 'app-home',
  standalone: true,
  providers: [MeterService],
  imports: [FormsModule, CommonModule,],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  constructor(private service: MeterService) {
  }

  serialNumber: string = '';
  message: string = 'Get latest meter reading.';
  alert: alert = 'info';


  meterReading: MeterReading = {
    id: '',
    readingTime: '',
    serialNumber: '',
    current: 0,
    endIndex: 0,
    voltage: 0
  }

  ngOnInit(): void {

  }
  search() {
    this.meterReading.id = '';
    if (this.serialNumber == '') {
      this.message = "Please enter a serial number.";
      this.alert = 'warning';
    } else {
      this.service.getMeterLastReading(this.serialNumber).pipe(catchError(e=>{
        this.message = "There is no meter reading with this serial number";
        this.alert = 'error';
        return of(null);
      })).subscribe(res => {
        if (res != null) {
          this.meterReading = res;
          this.message = 'Get latest meter reading.';
        }
      })
    }
  }
}
