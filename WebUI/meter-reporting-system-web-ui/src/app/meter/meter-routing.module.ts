import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MetersComponent } from './components/meters/meters.component';
import { MeterComponent } from './components/meter/meter.component';
import { provideHttpClient } from '@angular/common/http';

const routes: Routes = [
  { path:'', component:MetersComponent  },
  { path:':id', component:MeterComponent  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers:[provideHttpClient()]
})
export class MeterRoutingModule { }
