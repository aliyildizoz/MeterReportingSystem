
export interface ReportRequest {
  id: string;
  serialNumber:string;
  requestDate:string;
  status:Status;
}

export enum Status{
  preparing,
  completed
}
