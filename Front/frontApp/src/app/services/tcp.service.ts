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

  private writeLog(text: string, error: boolean = false): void {
    if (this._config.logging) {
      if (error) {
        console.error(text);
      } else {
        console.log(text);
      }
    }

  }

  async send(textMessage: string) {
    debugger;
    const promiseSocket = new window.promiseSocket.PromiseSocket(this.socket);
    await promiseSocket.connect(this.port, this.host);

    await promiseSocket.write(this.getLength(textMessage.length));
    await promiseSocket.write(textMessage);

    const content = await promiseSocket.readAll()

    await promiseSocket.end();
    promiseSocket.destroy();

    return JSON.parse(content);
  }

  /*sendMessage(textMessage: string) {
    return new Promise<TransferObject>((resolve, reject) => {
      let client = new window.net.Socket();
      client.connect(this._config.port, this._config.host, () => {
        this.writeLog(`Client connected to ${this._config.host}:${this._config.port}`);
        client.write(this.getLength(textMessage.length));
        client.write(textMessage);
        this.writeLog('Message was wrtitten');
      });

    let buffered = '';
    // const socket = net.createConnection({ port: 8000, host: 'localhost' });
    client.on('connect', () => {
      this.writeLog('TCP Connected');
      client.on('data', data => {
        console.log(data.length);
        buffered += data;
        // processReceived();
      });
    });

      client.on('close', () => {
        const dataObject: TransferObject = JSON.parse(`${buffered}`);
        resolve(dataObject);
        console.log('Client closed');
      });

      client.on('error', (err) => {
        console.error(err);
      });
    }
    );
  }*/

  /**
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
    // let length = Math.ceil((Math.log(messageLength) / Math.log(2)) / 8); // How much byte to store integer in the buffer
    // let buffer = new Buffer(length);
    // let arr = []; // Use to create the binary representation of the integer

    // while (messageLength > 0) {
    //   var temp = messageLength % 2;
    //   arr.push(temp);
    //   messageLength = Math.floor(messageLength / 2);
    // }

    // let counter = 0;
    // let total = 0;

    // for (let i = 0, j = arr.length; i < j; i++) {
    //   if (counter % 8 == 0 && counter > 0) { // Do we have a byte full ?
    //     buffer[length - 1] = total;
    //     total = 0;
    //     counter = 0;
    //     length--;
    //   }

    //   if (arr[i] == 1) { // bit is set
    //     total += Math.pow(2, counter);
    //   }
    //   counter++;
    // }

    // buffer[0] = total;
    // this.writeLog('CalcLength');
    return buffer;
    // return buffer;
  }
}
