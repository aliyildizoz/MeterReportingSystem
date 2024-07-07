import { Component } from '@angular/core';
import { ReportService } from '../../report.service';
import { Router, RouterModule } from '@angular/router';
import { ReportRequest, Status } from '../../models/report';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent {

  constructor(private service: ReportService, private router: Router) {
  }

  reports: ReportRequest[] = [];
  displayReports: ReportRequest[] = [];
  groupReports: ReportRequest[] = [];
  isGroup:boolean=false;
  ngOnInit(): void {
    this.service.getAllReport().subscribe(res => {
      this.reports = res;
      this.displayReports = res;
      this.groupReports = this.groupByLatestDate(res);
    })
  }


  groupByLatestDate(items: ReportRequest[]): ReportRequest[] {
    var grouped = items.reduce((acc, item) => {
      const group = item.serialNumber;
      if (!acc[group] || new Date(acc[group].requestDate) < new Date(item.requestDate)) {
        acc[group] = item;
      }
      return acc;
    }, {} as Record<string, ReportRequest>);

    return Object.values(grouped);
  }

  group(isGroup: boolean) {
    console.log(isGroup);
    this.isGroup = isGroup;
    if (isGroup) {
      this.displayReports = this.groupReports;
    } else {
      this.displayReports = this.reports;
    }
  }

  download(reportRequest: ReportRequest) {
    this.service.downloadReport(reportRequest.id).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${reportRequest.serialNumber}.xlsx`; // Adjust the file name as necessary
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    });
  }
  converStatus(status: number) {
    return status == 1 ? "Completed" : "Preparing";
  }
  goToDetails(id: string) {
    this.router.navigate(['/reports', id]);
  }
}
