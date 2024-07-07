import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MeterRoutingModule } from './meter-routing.module';
import { MeterComponent } from './components/meter/meter.component';
import { MetersComponent } from './components/meters/meters.component';
import { MeterService } from './meter.service';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MeterRoutingModule,
    MeterComponent,
    MetersComponent
  ],
  providers:[
    MeterService,
    provideHttpClient()
  ]
})
export class MeterModule { }
