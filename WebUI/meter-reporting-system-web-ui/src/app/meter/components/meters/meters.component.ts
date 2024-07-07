import { ReportRequest } from './../../../report/models/report';
import { Component, OnInit } from '@angular/core';
import { MeterService } from '../../meter.service';
import { MeterReading } from '../../models/meter';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ReportService } from '../../../report/report.service';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-meters',
  standalone: true,
  imports: [CommonModule, RouterModule, NgbToastModule],
  templateUrl: './meters.component.html',
  styleUrl: './meters.component.css'
})
export class MetersComponent implements OnInit {

  constructor(private service: MeterService, private reportService: ReportService, private router: Router, private toastrService: ToastrService) {
  }

  meters: MeterReading[] = [];
  displayMeters: MeterReading[] = [];
  groupMeters: MeterReading[] = [];

  reportRequestSerialNumber: string = '';
  isGroup: boolean = false;
  ngOnInit(): void {
    this.service.getAllMeter().subscribe(res => {
      this.meters = res;
      this.displayMeters = res;
      this.groupMeters = this.groupByLatestDate(res);
    })
  }

  groupByLatestDate(items: MeterReading[]): MeterReading[] {
    var grouped = items.reduce((acc, item) => {
      const group = item.serialNumber;
      if (!acc[group] || new Date(acc[group].readingTime) < new Date(item.readingTime)) {
        acc[group] = item;
      }
      return acc;
    }, {} as Record<string, MeterReading>);

    return Object.values(grouped);
  }

  group(isGroup: boolean) {
    console.log(isGroup);
    this.isGroup = isGroup;
    if (isGroup) {
      this.displayMeters = this.groupMeters;
    } else {
      this.displayMeters = this.meters;
    }
  }

  goToDetails(id: string) {
    this.router.navigate(['/meters', id]);
  }

  reportRequest(serialNumber: string) {
    this.reportRequestSerialNumber = serialNumber;
    this.reportService.createReportRequest(serialNumber).subscribe(res => {
      this.toastrService.info("Report Request Created for meter " + serialNumber, "Report Request").onTap.subscribe(() => {
        this.router.navigate(['/reports', res.id])
      });
    })
  }

}
