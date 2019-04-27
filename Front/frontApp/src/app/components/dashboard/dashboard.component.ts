import { Component, OnInit, ViewChild, ElementRef, OnDestroy, AfterContentInit } from '@angular/core';
import { TcpService } from '../../services/tcp.service';
import { PingService } from '../../services/ping.service';
import { DatePipe } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  providers: [DatePipe]
})
export class DashboardComponent implements AfterContentInit, OnDestroy {
  public lineChartData: Array<any> = [{ data: [], label: '' }];
  public lineChartLabels: string[] = [];
  private pingTimes: string[] = [];
  private pingResult: number[] = [];
  private timer: any;
  @ViewChild('baseChart') pingChart: BaseChartDirective;
  public lineChartOptions: any = {
    responsive: true,
    events: []
  };
  databases: any = [];
  constructor(
    private tcpService: TcpService,
    private pingService: PingService,
    public datepipe: DatePipe
  ) { }

  async ngAfterContentInit() {
    await this.loadDatabaseSizes();

    this.timer = setInterval(() => {
      this.updateChart();
    }, 3000);

  }

  ngOnDestroy() {
    clearInterval(this.timer);
  }

  async loadDatabaseSizes() {
    const res = await this.tcpService.send('/show databases size');
    if(res) this.databases = JSON.parse(res.Data.Message);
    /*this.tcpService.sendMessage().then(res => {
      this.databases = JSON.parse(res.Data.Message);
    });*/
  }

  updateChart() {
    this.pingService.pingServer().then(sec => {
      if (sec !== undefined) {
        if (this.lineChartData[0].data.length > 15) {
          this.lineChartData[0].data.shift();
          this.lineChartLabels.shift();
        }
        this.lineChartData[0].data.push(Math.floor(sec));
        this.lineChartLabels.push(this.datepipe.transform(Date.now(), 'mm:ss'));
        if (this.pingChart !== undefined) {
          this.pingChart.ngOnDestroy();
          this.pingChart.chart = this.pingChart.getChartBuilder(this.pingChart.ctx);
        }
      }
    });
  }
}
