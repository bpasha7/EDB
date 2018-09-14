import { Component, OnInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-view-table',
  templateUrl: './view-table.component.html',
  styleUrls: ['./view-table.component.css']
})
export class ViewTableComponent implements OnInit {
  displayedColumns: string[] = [];
  columnsToDisplay: string[] = [];
  data: any[] = [];
  message = '';
  db = '';
  table = '';
  constructor(
    private tcpService: TcpService,
    private route: ActivatedRoute,
    public snackBar: MatSnackBar
  ) { }

  ngOnInit() {

    this.route.params.subscribe(params => {
      this.db = params['db']
      this.table = params['table']
      this.tcpService.sendMessage("# " + this.db).then(res => {
        this.tcpService.sendMessage("select * from " + this.table).then(res2 => {
          this.data = [];
          const rows = res2.Data.Values;
          this.displayedColumns = res2.Data.Headers;
          rows.forEach(row => {
            let dataRow = {};
            for (let index = 0; index < this.displayedColumns.length; index++) {
              dataRow[this.displayedColumns[index]] = row[index];
            }
            this.data.push(dataRow);
          });
          this.columnsToDisplay = this.displayedColumns.slice();
          this.snackBar.open(res2.Time, 'Ок', {
            duration: 3500,
          });
        });
      });
    });
  }

  run() {
    this.tcpService.sendMessage("# " + this.db).then(res => {
      this.tcpService.sendMessage(this.message).then(res2 => {
        this.data = [];
        const rows = res2.Data.Values;
        this.displayedColumns = res2.Data.Headers;
        rows.forEach(row => {
          let dataRow = {};
          for (let index = 0; index < this.displayedColumns.length; index++) {
            dataRow[this.displayedColumns[index]] = row[index];
          }
          this.data.push(dataRow);
        });
        this.columnsToDisplay = this.displayedColumns.slice();
        this.snackBar.open(res2.Time, 'Ок', {
          duration: 3500,
        });
      });
    });
  }
}
