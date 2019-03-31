import { Injectable } from '@angular/core';
import { AppConfig } from '../app.config';
import { TransferObject } from '../models/transfer-object';
declare const Buffer
@Injectable({
  providedIn: 'root'
})
export class TcpService {

//  private readonly socket: any; 
  private host = '';
  private port = 0;
  private inProcces = false;
  constructor(
    private _config: AppConfig
  ) {
    this.host = this._config.host;
    this.port = this._config.port;
    //this.socket = new window.net.Socket();
    this.writeLog(`Init TcpService for ${this._config.host}:${this._config.port}`);
  }
  /**
   * Writin log if this option is ON into config file
   * @param text text message
   * @param error error flag
   */
  private writeLog(text: string, error: boolean = false): void {
    if (this._config.logging) {
      const message = `[${this.host}:${this.port}] ${text}`;
      if (error) {
        console.error(message);
      } else {
        console.log(message);
      }
    }
  }
  private async sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
  private async waitClient(){
    return new Promise<Boolean>(async (resolve) => {
      for(let i = 0; i <= 50; i++) {
        this.writeLog(`checking client status ${i} times.`);
        if(this.inProcces) await this.sleep(200);
        else break;
      }
      resolve(!this.inProcces);
    });
  }
  /**
   * Send text message throught tcp
   * @param textMessage 
   */
   async send(textMessage: string) {
    return new Promise<TransferObject>(async (resolve, reject) => {
      let recievedData = null;
      try {
        const clientReady = await this.waitClient();
        if (!clientReady) resolve(null);
        this.inProcces = true;
        const client = window.net.connect({port: this.port, host: this.host}, () => {
            this.writeLog(`connected.`);
            client.write(textMessage);
        });
        client.on('error', (exeption) => {
          this.writeLog(`handled error. ${exeption}`);
          resolve(null);
        });
        client.on('data', (data) => {
            recievedData = data;
            this.writeLog(`recieved ${data.length} bytes.`);
            client.end();
        });
        client.on('end', () => {
          this.writeLog(`disconnected.`);
          this.inProcces = false;
          if (recievedData) resolve(JSON.parse(recievedData));
          else resolve(null);
        });
      }
      catch(e) {
        this.writeLog(`promise error. ${e}`);
        this.inProcces = false;
        reject(e);
      } 
    });
   }
}
