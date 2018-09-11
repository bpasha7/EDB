import { Injectable } from '@angular/core';
import { AppConfig } from '../app.config';
import { TransferObject } from '../models/transfer-object';
import { Observable } from 'rxjs';

declare const Buffer
@Injectable({
  providedIn: 'root'
})
export class TcpService {

  private client: any;
  private host = '';
  private port = 0;
  constructor(
    private _config: AppConfig
  ) {
    this.writeLog('Init TcpService');
    this.client = new window.net.Socket();
  }

  private writeLog(text: string, error: boolean = false): void {
    if (this._config.logging) {
      if (error) {
        console.error(text);
      }
      else {
        console.log(text);
      }
    }

  }
  sendMessage(textMessage: string) {
    return new Promise<TransferObject>((resolve, reject) => {
      let client = new window.net.Socket();
      client.connect(this._config.port, this._config.host, () => {
        this.writeLog(`Client connected to ${this._config.host}:${this._config.port}`);
        client.write(this.getLength(textMessage.length));
        client.write(textMessage);
        this.writeLog('Message was wrtitten');
      })
      let recived = '';
      client.on('data', (data) => {
        console.log(`Client received: ${data}`);
        // const size = data;
        // while(recived.length != size)
        //   recived += data;
        client.destroy();
        const dataObject: TransferObject = JSON.parse(`${data}`);
        resolve(dataObject);
      });

      client.on('close', () => {
        console.log('Client closed');
      });

      client.on('error', (err) => {
        console.error(err);
      });
    }
    );
  }

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
