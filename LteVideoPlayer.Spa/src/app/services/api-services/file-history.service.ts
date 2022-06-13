import { Injectable } from '@angular/core';
import { IFileHistoryDto } from 'src/app/models/models';
import { ApiHttpService } from '../http/api-http.service';
import { ModelStateErrors } from '../http/ModelStateErrors';

@Injectable({
  providedIn: 'root',
})
export class FileHistoryService {
  private baseUri = 'FileHistory/';
  private lastWatchedVideo: IFileHistoryDto | null = null;

  constructor(private readonly httpClient: ApiHttpService) {}

  getFileHistoriesByUserProfile(
    userProfileId: number,
    callback: (videoFiles: IFileHistoryDto[]) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.get<IFileHistoryDto[]>(
      this.baseUri +
        'GetFileHistoriesByUserProfile?userProfileId=' +
        userProfileId,
      callback,
      errorCallback
    );
  }

  addUpdateWatchVideoFile(
    fileHistory: IFileHistoryDto,
    callback: (fileHistory: IFileHistoryDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IFileHistoryDto>(
      this.baseUri + 'AddUpdateFileHistory',
      fileHistory,
      (result) => {
        this.lastWatchedVideo = result;
        callback(result);
      },
      errorCallback
    );
  }

  getLastFileHistory(): IFileHistoryDto | null {
    return this.lastWatchedVideo;
  }
}
