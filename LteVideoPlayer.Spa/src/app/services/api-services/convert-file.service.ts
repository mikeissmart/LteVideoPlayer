import { Injectable } from '@angular/core';
import {
  IConvertFileDto,
  IConvertManyFileDto,
  ICreateConvertDto,
  ICreateManyConvertDto,
  IFileDto,
  IStringDto,
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

  getWorkingConvertFiles(
    callback: (convertVideoFiles: IConvertFileDto[]) => void
  ): void {
    this.httpClient.get<IConvertFileDto[]>(
      this.baseUri + 'WorkingConvertFiles',
      callback
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

  addManyConvert(
    convertFile: ICreateManyConvertDto,
    callback: (convertVideoFile: IConvertManyFileDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post(
      this.baseUri + 'AddManyConvert',
      convertFile,
      callback,
      errorCallback
    );
  }
}
