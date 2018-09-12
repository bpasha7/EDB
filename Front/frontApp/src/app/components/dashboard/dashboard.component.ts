import { Component, OnInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';
import { PingService } from '../../services/ping.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  providers: [DatePipe]
})
export class DashboardComponent implements OnInit {
  public lineChartData:Array<any> = [{data: [], label: ''}];
  public lineChartLabels: string[] = [];
  private pingTimes: string[] = [];
  private pingResult: number[] = [];
  public lineChartOptions:any = {
    responsive: true,
    events: []
  };
  databases: any = [];
  constructor(
    private tcpService: TcpService,
    private pingService: PingService,
    public datepipe: DatePipe
  ) { }

  ngOnInit() {
    this.loadDatabaseSizes();

    setInterval(() => {
      this.updateChart();
  }, 3000);

  }

  loadDatabaseSizes() {
    this.tcpService.sendMessage('/show databases size').then(res => {
      this.databases = JSON.parse(res.Data.Message);
    });
  }

  updateChart() {
    this.pingService.pingServer().then(sec => {
      if(sec !== undefined){
        // if(this.pingTimes.length > 15) {
        //   this.pingResult.shift();
        //   this.pingTimes.shift();
        // }
        // this.pingResult.push(Math.floor(sec));
        // this.pingTimes.push(this.datepipe.transform(Date.now(), 'mm:ss'));
        // this.lineChartData[0] = [{data: this.pingResult, label: 'Ping, ms'}];
        // this.lineChartLabels = this.pingTimes;
        if(this.lineChartData[0].data.length > 15) {
          this.lineChartData[0].data = [];
          this.lineChartLabels = [];
        }
        this.lineChartData[0].data.push(Math.floor(sec));
        this.lineChartLabels.push(this.datepipe.transform(Date.now(), 'mm:ss'));


        // let data = this.lineChartData[0].data;
        // data.push(this.counter++);
        // this.lineChartData = [{data: data, label: 'test'}];
        // this.lineChartLabels.push('f'+this.counter++);
      }
    })
  }

  // ngOnDestroy(){
  //   this.killTrigger.next();
  // }

}
