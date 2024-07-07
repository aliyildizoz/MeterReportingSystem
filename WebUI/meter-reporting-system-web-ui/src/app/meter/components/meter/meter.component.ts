import { alert } from './../../../home/home.component';
import { MeterReading } from './../../models/meter';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms'
import { ActivatedRoute } from '@angular/router';
import { MeterService } from '../../meter.service';
import { CommonModule } from '@angular/common';
import { catchError, of } from 'rxjs';
import { NgbDatepickerModule, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { faCalendarDays } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-meter',
  standalone: true,
  imports: [FormsModule, CommonModule, NgbDatepickerModule, FontAwesomeModule, ReactiveFormsModule],
  templateUrl: './meter.component.html',
  styleUrl: './meter.component.css'
})
export class MeterComponent implements OnInit {

  constructor(private route: ActivatedRoute, private service: MeterService) { }

  meterReading: MeterReading = {
    id: '',
    readingTime: '',
    serialNumber: '',
    current: 0,
    endIndex: 0,
    voltage: 0
  }
  message: string = 'Meter reading not found';
  alert: alert = 'error';
  isNew: boolean = true;
  valid: boolean = true;
  disable: boolean = true;
  faCalendar = faCalendarDays;
  serialNumber: FormControl = new FormControl('');
  dateNow: Date = new Date()
  readingTime: NgbDateStruct = { day: this.dateNow.getDate(), month: this.dateNow.getMonth(), year: this.dateNow.getFullYear() };

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      console.log(params['id']);
      if (params['id'] != 'new') {
        this.isNew = false;
        this.service.getMetersById(params['id']).subscribe(res => {
          if (res != null) {
            this.meterReading = res;
          }
        })
      }
    });

    this.serialNumber.valueChanges.subscribe(val => {
      console.log(this.serialNumber.dirty);
      if (!this.serialNumber.dirty) {
        return;
      }
      if (val.length > 8 || val == '') {
        this.valid = false;
        this.disable = true;
      } else {
        this.valid = true;
        this.disable = false;
      }
    })
  }


  save() {
    var date = new Date(this.readingTime.year, this.readingTime.month, this.readingTime.day);
    this.meterReading.readingTime = date.toISOString();
    console.log(this.meterReading);
    this.service.postMeter(this.meterReading).subscribe(res=>{
      this.meterReading=res;
      this.isNew=false;
    });
  }

}
