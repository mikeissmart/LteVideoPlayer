import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { IDirectoryInfo, IDirsAndFiles, IFile } from '../../models/models';
import { ApiHttpService } from '../http/api-http.service';
import { DirectoryEnum } from '../../models/model.enum';
import { DirPathFile } from '../../models/view-models';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class DirectoryService {
  router = inject(Router);

  private baseUri = 'Directory/';

  currentDirectory = signal<DirPathFile>(new DirPathFile());

  constructor(private readonly httpClient: ApiHttpService) {}

  getCopyCurrentDirectory(): DirPathFile {
    return JSON.parse(JSON.stringify(this.currentDirectory()));
  }

  updateCurrentDirectory(directory: DirPathFile): void {
    var params = {
      admin: directory.isAdmin ? directory.isAdmin : null,
      directory: directory.dir?.friendlyName ?? null,
      path: directory.path.length > 0 ? directory.path : null,
      file: directory.file.length > 0 ? directory.file : null,
    };

    this.router.navigate([''], { queryParams: params });
  }

  getDirectories(callback: (directories: IDirectoryInfo[]) => void): void {
    this.httpClient.get<IDirectoryInfo[]>(
      this.baseUri + 'GetDirectories',
      callback
    );
  }

  getDirsAndFiles(
    dirEnum: DirectoryEnum,
    path: string,
    callback: (dirsAndFiles: IDirsAndFiles) => void,
    errorCallback: () => void
  ): void {
    this.httpClient.get<IDirsAndFiles>(
      this.baseUri +
        (path.length > 0
          ? `GetDirsAndFiles?dirEnum=${dirEnum}&path=${path}`
          : `GetRootDirsAndFiles?dirEnum=${dirEnum}`),
      callback,
      errorCallback
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
