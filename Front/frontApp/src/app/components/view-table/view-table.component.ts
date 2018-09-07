import { Component, OnInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';

@Component({
  selector: 'app-view-table',
  templateUrl: './view-table.component.html',
  styleUrls: ['./view-table.component.css']
})
export class ViewTableComponent implements OnInit {
  displayedColumns: string[] = [];
  columnsToDisplay: string[] = [];
  data: any[] = []];
  constructor(private tcpService: TcpService) { }

  ngOnInit() {

    this.tcpService.sendMessage('/show databases size').then(res => {
      this.data = JSON.parse(res.Data.Message);
    });

  }

}
