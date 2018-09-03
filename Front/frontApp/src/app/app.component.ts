import { Component } from '@angular/core';
import { TcpService } from './services/tcp.service';
declare const Buffer
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'frontApp';
  message = '/info';
  constructor(
    private tcpService: TcpService
  ) {
    // this.run();
  }


  run() {
    // alert('test');
    // window.fs.writeFileSync('sample.txt', 'my data');
    this.tcpService.sendMessage(this.message);
  }
}
