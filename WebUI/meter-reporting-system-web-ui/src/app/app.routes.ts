import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LayoutComponent } from './layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    title: 'Home page',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component:HomeComponent },
      { path: 'meters', loadChildren: () => import('./meter/meter.module').then(m => m.MeterModule) },
      { path: 'reports', loadChildren: () => import('./report/report.module').then(m => m.ReportModule) }
    ]
  }
];
export default routes;
