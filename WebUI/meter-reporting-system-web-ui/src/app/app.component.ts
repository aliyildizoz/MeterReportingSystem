import { Component, OnInit } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import {HomeComponent} from './home/home.component';
import { LayoutComponent } from './layout/layout.component';
import { BrowserAnimationsModule, provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr, ToastrModule, ToastrService } from 'ngx-toastr';
import { RealtimeClientService } from './services/realtime-client.service';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgbToastModule, RouterModule],
  providers:[RealtimeClientService],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'meter-reporting-system-web-ui';


  constructor(private realtimeClientService:RealtimeClientService,private router: Router, private toastrService: ToastrService) {


  }
  ngOnInit(): void {
    this.realtimeClientService.reportNotification$.subscribe(n=>{
      this.toastrService.success("Report for meter " + n.serialNumber +" created. Click to go to report.", "Report Created").onTap.subscribe(()=>{
        this.router.navigate(['/reports', n.id]);
      })
    })
  }
}
