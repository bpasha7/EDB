import { Component, OnInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';
import { PingService } from '../../services/ping.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  public lineChartData:Array<any> = [
    {data: [0, 0], label: 'Ping'},
  ];
  public lineChartLabels:Array<Date> = [];
  public lineChartOptions:any = {
    responsive: true
  };
  data: any = [];
  constructor(
    private tcpService: TcpService,
    private pingService: PingService
  ) { }

  ngOnInit() {
    this.loadDatabaseSizes();
  }

  loadDatabaseSizes() {
    this.tcpService.sendMessage('/show databases size').then(res => {
      this.data = JSON.parse(res.Data.Message);
    });
    this.pingService.pingServer();
  }

}
