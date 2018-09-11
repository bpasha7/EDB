import { Injectable } from '@angular/core';
import { AppConfig } from '../app.config';

@Injectable({
  providedIn: 'root'
})
export class PingService {

  constructor(
    private _config: AppConfig,
  ) { }

  pingServer() {
    window.ping.promise.probe(this._config.host)
      .then(function (res) {
        console.log(res);
      });
  }
}
