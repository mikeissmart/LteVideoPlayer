import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { IDirectoryInfo, IDirsAndFiles, IFile } from '../../models/models';
import { ApiHttpService } from '../http/api-http.service';
import { DirectoryEnum } from '../../models/model.enum';

@Injectable({
  providedIn: 'root',
})
export class DirectoryService {
  private baseUri = 'Directory/';

  currentDirectory = signal<IDirectoryInfo | null>(null);
  currentPath = signal<string>('');
  currentFile = signal<string>('');

  constructor(private readonly httpClient: ApiHttpService) {}

  getDirectories(callback: (directories: IDirectoryInfo[]) => void): void {
    this.httpClient.get<IDirectoryInfo[]>(
      this.baseUri + 'GetDirectories',
      callback
    );
  }

  getDirsAndFiles(
    dirEnum: DirectoryEnum,
    path: string,
    callback: (dirsAndFiles: IDirsAndFiles) => void
  ): void {
    this.httpClient.get<IDirsAndFiles>(
      this.baseUri +
        (path.length > 0
          ? `GetDirsAndFiles?dirEnum=${dirEnum}&path=${path}`
          : `GetRootDirsAndFiles?dirEnum=${dirEnum}`),
      callback
    );
  }

  getNextFile(
    dirEnum: DirectoryEnum,
    file: IFile,
    callback: (nextFile: IFile | null) => void
  ): void {
    this.httpClient.post<IFile | null>(
      this.baseUri + `GetNextFile?dirEnum=${dirEnum}`,
      file,
      callback
    );
  }

  streamFileUrl(dirEnum: DirectoryEnum, file: IFile): string {
    return (
      environment.apiUrl +
      this.baseUri +
      `StreamFile?dirEnum=${dirEnum}&fullPath=${file.fullPath}`
    );
  }
}
