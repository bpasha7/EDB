import { Injectable } from '@angular/core';
import { AppConfig } from '../app.config';
import { TransferObject } from '../models/transfer-object';
import { Observable } from 'rxjs';

declare const Buffer
@Injectable({
  providedIn: 'root'
})
export class TcpService {

  private readonly socket: any; 
  private host = '';
  private port = 0;
  constructor(
    private _config: AppConfig
  ) {
    this.host = this._config.host;
    this.port = this._config.port;
    this.socket = new window.net.Socket();
    this.writeLog(`Init TcpService for ${this._config.host}:${this._config.port}`);
  }
  /**
   * Writin log if this option is ON into config file
   * @param text text message
   * @param error error flag
   */
  private writeLog(text: string, error: boolean = false): void {
    if (this._config.logging) {
      if (error) {
        console.error(text);
      } else {
        console.log(text);
      }
    }

  }
  /**
   * Send text message throught tcp
   * @param textMessage 
   */
  async send(textMessage: string) {
    const promiseSocket = new window.promiseSocket.PromiseSocket(this.socket);
    await promiseSocket.connect(this.port, this.host);

    //await promiseSocket.write(this.getLength(textMessage.length));
    await promiseSocket.write(textMessage);

    const content = await promiseSocket.readAll()

    await promiseSocket.end();
    promiseSocket.destroy();

    return JSON.parse(content);
  }
  /**NOT USES
   * Get bytes of message length
   * @param messageLength Message length
   */
  private getLength(messageLength: number): any {
    let buffer = new Buffer(4);
    let length = messageLength;
    // let bytes = Array();
    buffer[0] = length & (255);
    length = length >> 8;
    buffer[1] = length & (255);
    length = length >> 8;
    buffer[2] = length & (255);
    length = length >> 8;
    buffer[3] = length & (255);
    length = length >> 8;
    return buffer;
  }
}
