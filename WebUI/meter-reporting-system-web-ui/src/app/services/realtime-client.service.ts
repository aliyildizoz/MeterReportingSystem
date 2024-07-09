import { Injectable } from '@angular/core';
import { ReportNotification } from './model';
import * as signalR from "@microsoft/signalr";
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RealtimeClientService {

  private hubConnection: signalR.HubConnection;
  private reportNotificationSubject = new Subject<ReportNotification>();
  reportNotification$: Observable<ReportNotification> = this.reportNotificationSubject.asObservable();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:3516/report-notification-hub') // Replace with your SignalR hub URL
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connected to SignalR hub'))
      .catch(err => console.error('Error connecting to SignalR hub:', err));

    this.hubConnection.on('ReceiveNotification', (serialNumber: string, id: string) => {
      this.reportNotificationSubject.next({ id: id, serialNumber: serialNumber });
    });
  }

}
