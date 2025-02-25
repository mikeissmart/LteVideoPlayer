import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import {
  IRemoteVideoInfo,
  IRemoteSetSeek,
  IRemoteMoveSeek,
  IRemoteSetVolume,
  IRemote,
} from '../../models/models';

@Injectable({
  providedIn: 'root',
})
export class RemoteHubService {
  private connection!: signalR.HubConnection;

  isConnected = signal<boolean>(false);
  myChannel = signal<number>(0);

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl + 'remotehub')
      .withAutomaticReconnect()
      .build();
    this.connection.onreconnecting(() => {
      console.log('RemoteHub reconnecting');
      this.isConnected.set(false);
    });
    this.connection.onreconnected(() => {
      console.log('RemoteHub reconnected');
      this.isConnected.set(true);
    });
  }

  connect(callback: () => void, errorCallback?: (error: any) => void): void {
    console.log('RemoteHub connecting');
    this.connection
      .start()
      .then(() => {
        console.log('RemoteHub connected');
        this.isConnected.set(true);
        callback();
      })
      .catch((error) => {
        console.log('RemoteHub connection failed');
        this.isConnected.set(false);
        this.myChannel.set(0);
        if (errorCallback) {
          errorCallback(error);
        }
      });
  }

  disconnect(
    callback?: () => void,
    errorCallback?: (error: any) => void
  ): void {
    console.log('RemoteHub disconnecting');
    this.connection
      .stop()
      .then(() => {
        console.log('RemoteHub disconnected');
        this.isConnected.set(false);
        this.myChannel.set(0);
        if (callback) {
          callback();
        }
      })
      .catch((error) => {
        console.log('RemoteHub disconnect failed');
        this.isConnected.set(false);
        this.myChannel.set(0);
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

  sendVideoInfo(data: IRemoteVideoInfo): void {
    this.connection.invoke('VideoInfo', data);
  }

  sendAskForVideoInfo(data: IRemote): void {
    this.connection.invoke('AskForVideoInfo', data);
  }

  sendSetSeek(data: IRemoteSetSeek): void {
    this.connection.invoke('SetSeek', data);
  }

  sendMoveSeek(data: IRemoteMoveSeek): void {
    this.connection.invoke('MoveSeek', data);
  }

  sendSetVolume(data: IRemoteSetVolume): void {
    this.connection.invoke('SetVolume', data);
  }

  sendPlayPause(data: IRemote): void {
    this.connection.invoke('PlayPause', data);
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

  receiveVideoInfo(callback: (data: IRemoteVideoInfo) => void): void {
    this.connection.on('ReceiveVideoInfo', (data) => callback(data));
  }

  receiveAskForVideoInfo(callback: (data: IRemote) => void): void {
    this.connection.on('ReceiveAskForVideoInfo', (data) => callback(data));
  }

  receiveSetSeek(callback: (data: IRemoteSetSeek) => void): void {
    this.connection.on('ReceiveSetSeek', (data) => callback(data));
  }

  receiveMoveSeek(callback: (data: IRemoteMoveSeek) => void): void {
    this.connection.on('ReceiveMoveSeek', (data) => callback(data));
  }

  receivePlayPause(callback: (data: IRemote) => void): void {
    this.connection.on('ReceivePlayPause', (data) => callback(data));
  }

  receiveSetVolume(callback: (data: IRemoteSetVolume) => void): void {
    this.connection.on('ReceiveSetVolume', (data) => callback(data));
  }

  receiveError(callback: (error: string) => void): void {
    this.connection.on('Error', (error) => callback(error));
  }
}
