import { Injectable } from '@angular/core';
import {
  IConvertFileDto,
  ICreateConvertDto,
  IFileDto,
} from 'src/app/models/models';
import { ApiHttpService } from '../http/api-http.service';
import { ModelStateErrors } from '../http/ModelStateErrors';

@Injectable({
  providedIn: 'root',
})
export class ConvertFileService {
  private baseUri = 'ConvertFile/';

  constructor(private readonly httpClient: ApiHttpService) {}

  getAllConvertFiles(
    callback: (convertVideoFiles: IConvertFileDto[]) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.get<IConvertFileDto[]>(
      this.baseUri + 'GetAllConvertFiles',
      callback,
      errorCallback
    );
  }

  getConvertFileByOriginalFile(
    file: IFileDto,
    callback: (convertVideoFiles: IConvertFileDto[]) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IConvertFileDto[]>(
      this.baseUri + 'GetConvertFileByOriginalFile',
      file,
      callback,
      errorCallback
    );
  }

  addConvert(
    convertFile: ICreateConvertDto,
    callback: (convertVideoFile: IConvertFileDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post(
      this.baseUri + 'AddConvert',
      convertFile,
      callback,
      errorCallback
    );
  }
}
