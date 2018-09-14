import { Time } from "@angular/common";

export class ErrorData {
    Message: string;
}
export class ResultData {
    DataType: number;
    Message: string;
    Values: any[];
    Headers: string[];
}

export class TransferObject {
    Error: ErrorData;
    Data: ResultData;
    Time: string;
}
