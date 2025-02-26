import { Injectable } from '@angular/core';
import {
  IConvertFile,
  ICreateDirectoryConvert,
  ICreateFileConvert,
  IFile,
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
    callback: (convertVideoFiles: IConvertFile[]) => void
  ): void {
    this.httpClient.get<IConvertFile[]>(
      this.baseUri + `GetAllConvertFiles`,
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

  addConvertFile(
    dirEnum: DirectoryEnum,
    convertFile: ICreateFileConvert,
    callback: (convertVideoFile: IConvertFile) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post(
      this.baseUri + `AddConvertFile?dirEnum=${dirEnum}`,
      convertFile,
      callback,
      errorCallback
    );
  }

  addConvertDirectory(
    dirEnum: DirectoryEnum,
    convertFile: ICreateDirectoryConvert,
    callback: (convertVideoFile: ICreateDirectoryConvert) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post(
      this.baseUri + `AddConvertDirectory?dirEnum=${dirEnum}`,
      convertFile,
      callback,
      errorCallback
    );
  }

  getCurrentConvertFile(
    callback: (convertVideoFile: IConvertFile | null) => void
  ): void {
    this.httpClient.get<IConvertFile | null>(
      this.baseUri + 'GetCurrentConvertFile',
      callback
    );
  }

  getVideoMetadata(
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
