import { Component, OnInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  data: any = [];
  constructor(
    private tcpService: TcpService
  ) { }

  ngOnInit() {
    this.loadDatabaseSizes();
  }

  loadDatabaseSizes() {
    this.tcpService.sendMessage('/show databases size').then(res => {
      this.data = JSON.parse(res.Data.Message);
    });
  }

}
