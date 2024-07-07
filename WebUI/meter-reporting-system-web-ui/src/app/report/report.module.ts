import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportRoutingModule } from './report-routing.module';
import { ReportComponent } from './components/report/report.component';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ReportRoutingModule,
    ReportComponent,
    ReportComponent
  ]
})
export class ReportModule { }
