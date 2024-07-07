import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReportService } from '../../report.service';
import { ReportRequest, Status } from '../../models/report';
import { alert } from './../../../home/home.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './report.component.html',
  styleUrl: './report.component.css'
})
export class ReportComponent {
  constructor(private route: ActivatedRoute, private service: ReportService) { }

  reportRequest: ReportRequest = {
    id: '',
    serialNumber: '',
    requestDate: '',
    status: Status.preparing
  }
  message: string = 'Meter reading not found';
  alert: alert = 'error';

  converStatus(status: number) {
    return status == 1 ? "Completed" : "Preparing";
  }
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.service.getReportById(params['id']).subscribe(res => {
        if (res != null) {
          this.reportRequest = res;
        }
      })
    });
  }

  download() {
    this.service.downloadReport(this.reportRequest.id).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${this.reportRequest.serialNumber}.xlsx`; // Adjust the file name as necessary
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    });
  }
}
