import { Injectable } from '@angular/core';
import { IStringDto } from 'src/app/models/models';
import { ApiHttpService } from '../http/api-http.service';

@Injectable({
  providedIn: 'root',
})
export class ShareConnectService {
  private baseUri = 'ShareConnect/';

  constructor(private readonly httpClient: ApiHttpService) {}

  getShareConnectStatus(callback: (status: string) => void): void {
    this.httpClient.get<IStringDto>(
      this.baseUri + 'ShareConnectStatus',
      (result) => callback(result.data!)
    );
  }
}
