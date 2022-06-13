import { Injectable } from '@angular/core';
import { IDirDto, IDirsAndFilesDto, IFileDto } from 'src/app/models/models';
import { environment } from 'src/environments/environment';
import { ApiHttpService } from '../http/api-http.service';
import { ModelStateErrors } from '../http/ModelStateErrors';

@Injectable({
  providedIn: 'root',
})
export class DirectoryService {
  private baseUri = 'Directory/';

  constructor(private readonly httpClient: ApiHttpService) {}

  getDirsAndFiles(
    dirPathName: string,
    isStaging: boolean,
    callback: (dirsAndFiles: IDirsAndFilesDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.get<IDirsAndFilesDto>(
      this.baseUri +
        (dirPathName.length > 0
          ? `GetDirsAndFiles?dirPathName=${dirPathName}&isStaging=${isStaging}`
          : `GetRootDirsAndFiles?isStaging=${isStaging}`),
      callback,
      errorCallback
    );
  }

  getNextFile(
    file: IFileDto,
    isStaging: boolean,
    callback: (nextFile: IFileDto | null) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IFileDto | null>(
      this.baseUri + `GetNextFile?isStaging=${isStaging}`,
      file,
      callback,
      errorCallback
    );
  }

  streamFileUrl(file: IFileDto): string {
    return (
      environment.apiUrl +
      this.baseUri +
      `StreamFile?filePathName=${file.filePathName}`
    );
  }
}
