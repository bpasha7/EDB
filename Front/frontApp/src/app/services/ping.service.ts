import { Injectable } from '@angular/core';
import { AppConfig } from '../app.config';

@Injectable({
  providedIn: 'root'
})
export class PingService {

  constructor(
    private _config: AppConfig,
  ) { }

  pingServer():Promise<number> {
    return new Promise<number>((resolve, reject) => {
      window.ping.ping(
        { address: this._config.host, port: 80, attempts: 5 }, function (data) {
          console.log(data);
          if (data[0].results.length >= 1) {
            resolve(data[0].results[2].time);
          }
          else {
            reject(-1);
          }
        });
    });
  }
}
