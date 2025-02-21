import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { IString, IFile, IThumbnailError } from '../../models/models';
import { ApiHttpService } from '../http/api-http.service';
import { DirectoryEnum } from '../../models/model.enum';

@Injectable({
  providedIn: 'root',
})
export class ThumbnailService {
  private baseUri = 'Thumbnail/';

  constructor(private readonly httpClient: ApiHttpService) {}

  getAllThumbnailErrors(
    dirEnum: DirectoryEnum,
    callback: (thumbnailErrors: IThumbnailError[]) => void
  ): void {
    this.httpClient.get<IThumbnailError[]>(
      this.baseUri + `GetAllThumbnailErrors?dirEnum=${dirEnum}`,
      callback
    );
  }

  getFolderThumbnailUrl(dirEnum: DirectoryEnum, fullPath: string): string {
    return (
      environment.apiUrl +
      this.baseUri +
      `GetFolderThunbmail?dirEnum=${dirEnum}&fullPath=${fullPath}`
    );
  }

  getFileThumbnailUrl(dirEnum: DirectoryEnum, fullPath: string): string {
    return (
      environment.apiUrl +
      this.baseUri +
      `GetFileThumbnail?dirEnum=${dirEnum}&fullPath=${fullPath}`
    );
  }

  hasFileThumbnail(
    dirEnum: DirectoryEnum,
    fullPath: string,
    callback: (exists: boolean) => void
  ): void {
    this.httpClient.get<boolean>(
      this.baseUri + `HasFileThumbnail?dirEnum=${dirEnum}&fullPath=${fullPath}`,
      callback
    );
  }

  deleteThumbnail(
    dirEnum: DirectoryEnum,
    fullPath: string,
    callback: () => void
  ): void {
    this.httpClient.post<IString>(
      this.baseUri + `DeleteThumbnail?dirEnum=${dirEnum}`,
      { data: fullPath } as IString,
      callback
    );
  }

  deleteThumbnailError(
    dirEnum: DirectoryEnum,
    file: IFile,
    callback: () => void
  ): void {
    this.httpClient.post<IFile>(
      this.baseUri + `DeleteThumbnailError?dirEnum=${dirEnum}`,
      file,
      callback
    );
  }

  deleteManyThumbnailErrors(
    dirEnum: DirectoryEnum,
    files: IFile[],
    callback: () => void
  ): void {
    this.httpClient.post<IFile[]>(
      this.baseUri + `DeleteManyThumbnailErrors?dirEnum=${dirEnum}`,
      files,
      callback
    );
  }

  GetCurrentThumbnail(callback: (workingThumbnail: IString) => void): void {
    this.httpClient.get<IString>(
      this.baseUri + `GetCurrentThumbnail`,
      callback
    );
  }
}
