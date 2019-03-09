import { Injectable } from '@angular/core';
import { AppConfig } from '../app.config';

@Injectable({
  providedIn: 'root'
})
export class PingService {

  private socket: any;
  private host = '';
  constructor(
    private _config: AppConfig
  ) {
    this.host = this._config.host;
  }

  pingServer():Promise<number> {
    return new Promise<number>((resolve, reject) => {
      window.ping.ping(
        { address: this.host, port: 80, attempts: 3 }, function (data) {
          console.log(data);
          if (data[0].results.length >= 1) {
            resolve(data[0].results[2].time);
          } else {
            reject(-1);
          }
        });
    });
  }
}
