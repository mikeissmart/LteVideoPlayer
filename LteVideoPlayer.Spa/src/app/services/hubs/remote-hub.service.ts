import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  IRemoteData_MoveSeekDto,
  IRemoteData_PauseDto,
  IRemoteData_PlayDto,
  IRemoteData_SetSeekDto,
  IRemoteData_SetVolumeDto,
  IRemoteData_VideoInfoDto,
} from 'src/app/models/models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class RemoteHubService {
  private connection: signalR.HubConnection;
  private readonly stateBehSub$: BehaviorSubject<boolean>;
  private stateObs$: Observable<boolean>;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl + 'remotehub')
      .build();
    this.stateBehSub$ = new BehaviorSubject<boolean>(false);
    this.stateObs$ = this.stateBehSub$.asObservable();
  }

  observable(): Observable<boolean> {
    return this.stateObs$;
  }

  connect(callback: () => void, errorCallback?: (error: any) => void): void {
    this.connection
      .start()
      .then(() => {
        this.stateBehSub$.next(true);
        callback();
      })
      .catch((error) => {
        this.stateBehSub$.next(false);
        if (errorCallback) {
          errorCallback(error);
        }
      });
  }

  disconnect(
    callback?: () => void,
    errorCallback?: (error: any) => void
  ): void {
    this.connection
      .stop()
      .then(() => {
        this.stateBehSub$.next(false);
        if (callback) {
          callback();
        }
      })
      .catch((error) => {
        this.stateBehSub$.next(false);
        if (errorCallback) {
          errorCallback(error);
        }
      });
  }

  sendJoinProfile(profile: string): void {
    this.connection.invoke('JoinProfile', profile);
  }

  sendLeaveProfile(): void {
    this.connection.invoke('LeaveProfile');
  }

  sendGetChannels(): void {
    this.connection.invoke('GetChannels');
  }

  sendResetChannel(): void {
    this.connection.invoke('ResetChannel');
  }

  sendVideoInfo(data: IRemoteData_VideoInfoDto): void {
    this.connection.invoke('VideoInfo', data);
  }

  sendSetSeek(data: IRemoteData_SetSeekDto): void {
    this.connection.invoke('SetSeek', data);
  }

  sendMoveSeek(data: IRemoteData_MoveSeekDto): void {
    this.connection.invoke('MoveSeek', data);
  }

  sendVideoPause(data: IRemoteData_PauseDto): void {
    this.connection.invoke('VideoPause', data);
  }

  sendVideoPlay(data: IRemoteData_PlayDto): void {
    this.connection.invoke('VideoPlay', data);
  }

  sendSetVolume(data: IRemoteData_SetVolumeDto): void {
    this.connection.invoke('SetVolume', data);
  }

  receiveAddChannel(callback: (channel: number) => void): void {
    this.connection.on('AddChannel', (channel) => callback(channel));
  }

  receiveYourChannel(callback: (channel: number) => void): void {
    this.connection.on('YourChannel', (channel) => callback(channel));
  }

  receiveRemoveChannel(callback: (channel: number) => void): void {
    this.connection.on('RemoveChannel', (channel) => callback(channel));
  }

  receiveVideoInfo(callback: (data: IRemoteData_VideoInfoDto) => void): void {
    this.connection.on('ReceiveVideoInfo', (data) => callback(data));
  }

  receiveSetSeek(callback: (data: IRemoteData_SetSeekDto) => void): void {
    this.connection.on('ReceiveSetSeek', (data) => callback(data));
  }

  receiveMoveSeek(callback: (data: IRemoteData_MoveSeekDto) => void): void {
    this.connection.on('ReceiveMoveSeek', (data) => callback(data));
  }

  receiveVideoPause(callback: (data: IRemoteData_PauseDto) => void): void {
    this.connection.on('ReceiveVideoPause', (data) => callback(data));
  }

  receiveVideoPlay(callback: (data: IRemoteData_PlayDto) => void): void {
    this.connection.on('ReceiveVideoPlay', (data) => callback(data));
  }

  receiveSetVolume(callback: (data: IRemoteData_SetVolumeDto) => void): void {
    this.connection.on('ReceiveSetVolume', (data) => callback(data));
  }

  receiveError(callback: (error: string) => void): void {
    this.connection.on('Error', (error) => callback(error));
  }
}
