import { Injectable } from '@angular/core';
import {
  IConvertFile,
  IFile,
  ICreateConvert,
  ICreateManyConvert,
  IConvertManyFile,
  IMetadata,
} from '../../models/models';
import { ModelStateErrors } from '../../models/ModelStateErrors';
import { ApiHttpService } from '../http/api-http.service';
import { DirectoryEnum } from '../../models/model.enum';

@Injectable({
  providedIn: 'root',
})
export class ConvertFileService {
  private baseUri = 'ConvertFile/';

  constructor(private readonly httpClient: ApiHttpService) {}

  getAllConvertFiles(
    dirEnum: DirectoryEnum,
    callback: (convertVideoFiles: IConvertFile[]) => void
  ): void {
    this.httpClient.get<IConvertFile[]>(
      this.baseUri + `GetAllConvertFiles?dirEnum=${dirEnum}`,
      callback
    );
  }

  getConvertFileByOriginalFile(
    dirEnum: DirectoryEnum,
    file: IFile,
    callback: (convertVideoFiles: IConvertFile[]) => void
  ): void {
    this.httpClient.post<IConvertFile[]>(
      this.baseUri + `GetConvertFileByOriginalFile?dirEnum=${dirEnum}`,
      file,
      callback
    );
  }

  addConvert(
    dirEnum: DirectoryEnum,
    convertFile: ICreateConvert,
    callback: (convertVideoFile: IConvertFile) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post(
      this.baseUri + `AddConvert?dirEnum=${dirEnum}`,
      convertFile,
      callback,
      errorCallback
    );
  }

  addManyConvert(
    dirEnum: DirectoryEnum,
    convertFile: ICreateManyConvert,
    callback: (convertVideoFile: IConvertManyFile) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post(
      this.baseUri + `AddManyConvert?dirEnum=${dirEnum}`,
      convertFile,
      callback,
      errorCallback
    );
  }

  getCurrentConvertFile(
    callback: (convertVideoFiles: IConvertFile) => void
  ): void {
    this.httpClient.get<IConvertFile>(
      this.baseUri + 'GetCurrentConvertFile',
      callback
    );
  }

  getVideoMeta(
    dirEnum: DirectoryEnum,
    file: IFile,
    callback: (meta: IMetadata) => void
  ): void {
    this.httpClient.get<IMetadata>(
      this.baseUri +
        `GetVideoMeta?dirEnum=${dirEnum}&fullPath=${file.fullPath}`,
      callback
    );
  }
}
