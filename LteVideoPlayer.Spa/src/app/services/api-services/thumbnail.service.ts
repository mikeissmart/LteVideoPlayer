import { Injectable } from '@angular/core';
import { IStringDto, IFileDto, IMetaDataDto, IThumbnailErrorDto } from 'src/app/models/models';
import { environment } from 'src/environments/environment';
import { ApiHttpService } from '../http/api-http.service';

@Injectable({
  providedIn: 'root'
})
export class ThumbnailService {
  private baseUri = 'Thumbnail/';

  constructor(private readonly httpClient: ApiHttpService) {}

  getFolderThumbnailUrl(subpath: string): string {
    return (
      environment.apiUrl +
      this.baseUri +
      `GetFolderThunbmail?filePathName=${subpath}`
    );
  }

  getFileThumbnailUrl(subpath: string): string {
    return (
      environment.apiUrl +
      this.baseUri +
      `GetFileThumbnail?filePathName=${subpath}`
    );
  }

  hasFileThumbnail(subpath: string, callback: (exists: boolean) => void): void {
    this.httpClient.get<boolean>(
      this.baseUri + `HasFileThumbnail?filePathName=${subpath}`,
      callback
    );
  }

  deleteThumbnail(subpath: string, callback: () => void): void {
    this.httpClient.post<IStringDto>(
      this.baseUri + `DeleteThumbnail`,
      { data: subpath } as IStringDto,
      callback
    );
  }

  getVideoMeta(
    file: IFileDto,
    isStaging: boolean,
    callback: (meta: IMetaDataDto) => void
  ): void {
    this.httpClient.get<IMetaDataDto>(
      this.baseUri +
        `GetVideoMeta?filePathName=${file.filePathName}&isStaging=${isStaging}`,
      callback
    );
  }

  getWorkingThumbnail(callback: (workingThumbnail: IStringDto) => void): void {
    this.httpClient.get<IStringDto>(
      this.baseUri + `GetWorkingThumbnail`,
      callback
    );
  }

  getThumbnailErrors(callback: (thumbnailErrors: IThumbnailErrorDto[]) => void): void {
    this.httpClient.get<IThumbnailErrorDto[]>(
      this.baseUri + `GetThumbnailErrors`,
      callback
    );
  }

  deleteThumbnailError(file: IFileDto, callback: () => void): void {
    this.httpClient.post<IFileDto>(
      this.baseUri + `DeleteThumbnailError`,
      file,
      callback
    );
  }

  deleteManyThumbnailErrors(files: IFileDto[], callback: () => void): void {
    this.httpClient.post<IFileDto[]>(
      this.baseUri + `DeleteManyThumbnailErrors`,
      files,
      callback
    );
  }
}
