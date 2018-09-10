import { Component, OnInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-view-table',
  templateUrl: './view-table.component.html',
  styleUrls: ['./view-table.component.css']
})
export class ViewTableComponent implements OnInit {
  displayedColumns: string[] = [];
  columnsToDisplay: string[] = [];
  data: any[] = [];
  db = '';
  table = '';
  constructor(
    private tcpService: TcpService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {

    this.route.params.subscribe(params => {
      this.db = params['db']
      this.table = params['table']
      this.tcpService.sendMessage("# " + this.db).then(res => {
        this.tcpService.sendMessage("select * from emp").then(res2 => {
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
        });
      });
    });
  }
}
