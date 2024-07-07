import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportsComponent } from './components/reports/reports.component';
import { ReportComponent } from './components/report/report.component';

const routes: Routes = [
  { path:'', component:ReportsComponent  },
  { path:':id', component:ReportComponent  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
