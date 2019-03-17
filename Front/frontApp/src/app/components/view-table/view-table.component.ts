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

  async ngOnInit() {

    this.route.params.subscribe(params => {
      this.db = params['db'];
      this.table = params['table'];
    });
    
    await this.executeCommand(`select * from ${this.db}.${this.table}`);
  }

  async executeCommand(command) {
    debugger;
    //const res1 = await this.tcpService.send('# ' + this.db);

    const data = await this.tcpService.send(command);

    debugger;
    this.data = [];
    const rows = data.Data.Values;
    this.displayedColumns = data.Data.Headers;
    rows.forEach(row => {
      const dataRow = {};
      for (let index = 0; index < this.displayedColumns.length; index++) {
        dataRow[this.displayedColumns[index]] = row[index];
      }
      this.data.push(dataRow);
    });
    this.columnsToDisplay = this.displayedColumns.slice();
    this.snackBar.open(data.Time, 'Ок', {
      duration: 3500,
    });
  }

  async run() {
    await this.executeCommand(this.message);
    /*await this.tcpService.sendMessage('# ' + this.db);
    const data = await this.tcpService.sendMessage(this.message);

    if (data.Error !== null ) {
      this.snackBar.open(data.Error.Message, 'Ок', {
        duration: 3500,
      });
    }
    if (data.Data.DataType === 2) {
      this.data = [];
      const rows = data.Data.Values;
      this.displayedColumns = data.Data.Headers;
      rows.forEach(row => {
        let dataRow = {};
        for (let index = 0; index < this.displayedColumns.length; index++) {
          dataRow[this.displayedColumns[index]] = row[index];
        }
        this.data.push(dataRow);
      });
      this.columnsToDisplay = this.displayedColumns.slice();
      this.snackBar.open(data.Time, 'Ок', {
        duration: 3500,
      });
    } else {
      this.snackBar.open(data.Data.Message, 'Ок', {
        duration: 3500,
      });
    }*/

    // this.tcpService.sendMessage('# ' + this.db).then(res => {
    //   this.tcpService.sendMessage(this.message).then(res2 => {
    //     if (res2.Error !== null ) {
    //       this.snackBar.open(res2.Error.Message, 'Ок', {
    //         duration: 3500,
    //       });
    //     }
    //     if (res2.Data.DataType === 2) {
    //       this.data = [];
    //       const rows = res2.Data.Values;
    //       this.displayedColumns = res2.Data.Headers;
    //       rows.forEach(row => {
    //         let dataRow = {};
    //         for (let index = 0; index < this.displayedColumns.length; index++) {
    //           dataRow[this.displayedColumns[index]] = row[index];
    //         }
    //         this.data.push(dataRow);
    //       });
    //       this.columnsToDisplay = this.displayedColumns.slice();
    //       this.snackBar.open(res2.Time, 'Ок', {
    //         duration: 3500,
    //       });
    //     } else {
    //       this.snackBar.open(res2.Data.Message, 'Ок', {
    //         duration: 3500,
    //       });
    //     }

    //   });
    // });
  }
}
