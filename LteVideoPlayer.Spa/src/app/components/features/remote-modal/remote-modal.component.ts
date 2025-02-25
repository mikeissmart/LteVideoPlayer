import {
  Component,
  effect,
  inject,
  OnDestroy,
  OnInit,
  output,
  ViewChild,
} from '@angular/core';
import { ModalComponent } from '../../common/modal/modal.component';
import { RemoteHubService } from '../../../services/hubs/remote-hub.service';
import { UserProfileService } from '../../../services/api-services/user-profile.service';
import {
  IRemote,
  IRemoteMoveSeek,
  IRemoteSetSeek,
  IRemoteSetVolume,
  IRemoteVideoInfo,
  IUserProfile,
} from '../../../models/models';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-remote-modal',
  imports: [ModalComponent, FormsModule],
  templateUrl: './remote-modal.component.html',
  styleUrl: './remote-modal.component.scss',
})
export class RemoteModalComponent implements OnInit, OnDestroy {
  remoteHubService = inject(RemoteHubService);
  userProfileService = inject(UserProfileService);

  closeOutput = output({
    alias: 'close',
  });

  @ViewChild('modal')
  protected _modal!: ModalComponent;

  protected _userProfile: IUserProfile | null = null;
  protected _data = new Data();

  open(): void {
    this._modal.open();
  }

  close(): void {
    this._modal.close();
  }

  protected onClose(): void {
    this.closeOutput.emit();
  }

  constructor() {
    effect(() => {
      const curUserProfile = this.userProfileService.currentUserProfile();
      if (this._userProfile != null) {
        this.remoteHubService.disconnect();
      }
      this._userProfile = curUserProfile;
      this._data = new Data();
      if (this._userProfile != null) {
        this.connectRemote();
      }
    });
    effect(() => {
      this._data.isConnected = this.remoteHubService.isConnected();
    });
  }

  ngOnInit(): void {
    this.remoteHubService.receiveYourChannel((channel) =>
      this.remoteHubService.myChannel.set(channel)
    );
    this.remoteHubService.receiveAddChannel((channel) => {
      if (!this._data.otherChannels.includes(channel)) {
        this._data.otherChannels.push(channel);
        this._data.otherChannels.sort();
      }
    });
    this.remoteHubService.receiveRemoveChannel((channel) => {
      this._data.otherChannels = this._data.otherChannels.filter(
        (x) => x !== channel
      );
      this._data.otherChannels.sort();
    });
    this.remoteHubService.receiveVideoInfo(
      (result) => (this._data.videoInfo = result)
    );
    this.remoteHubService.receiveError((error) =>
      console.error(`RemoteComponent: ${error}`)
    );
  }

  ngOnDestroy(): void {
    this._data.loopAskForVideoInfo = false;
    this.remoteHubService.disconnect();
  }

  connectRemote(): void {
    this.remoteHubService.connect(() =>
      this.remoteHubService.sendJoinProfile(this._userProfile!.name)
    );
  }

  onReconnectRemote(): void {
    this.remoteHubService.disconnect(() => this.connectRemote());
  }

  onResetChannel(): void {
    this.remoteHubService.sendResetChannel();
  }

  onRefreshChannels(): void {
    this._data.otherChannels = [];
    this.remoteHubService.sendGetChannels();
  }

  onRefreshVideoInfo(): void {
    this.remoteHubService.sendAskForVideoInfo({
      profile: this._userProfile!.name,
      fromChannel: this.remoteHubService.myChannel(),
      toChannel: this._data.selectedChannel,
    } as IRemote);
  }

  onChannelChange(channel: number): void {
    this._data.selectedChannel = channel;

    if (this._data.selectedChannel == 0) {
      this._data.videoInfo = null;
    } else {
      this._data.loopAskForVideoInfo = true;
      this.sendAskVideoInfoSetTimeOut();
    }
  }

  onVolumeChange(value: number): void {
    this._data.videoInfo!.volume = value;
    this.remoteHubService.sendSetVolume({
      profile: this._userProfile!.name,
      fromChannel: this.remoteHubService.myChannel(),
      toChannel: this._data.selectedChannel,
      volume: value,
    } as IRemoteSetVolume);
  }

  onSetSeek(seekPercent: number): void {
    this.remoteHubService.sendSetSeek({
      profile: this._userProfile!.name,
      fromChannel: this.remoteHubService.myChannel(),
      toChannel: this._data.selectedChannel,
      seekPercent: seekPercent,
    } as IRemoteSetSeek);
  }

  onSkip(seek: number): void {
    this.remoteHubService.sendMoveSeek({
      profile: this._userProfile!.name,
      fromChannel: this.remoteHubService.myChannel(),
      toChannel: this._data.selectedChannel,
      seek: seek,
    } as IRemoteMoveSeek);
  }

  onPlayPause(): void {
    this.remoteHubService.sendPlayPause({
      profile: this._userProfile!.name,
      fromChannel: this.remoteHubService.myChannel(),
      toChannel: this._data.selectedChannel,
    } as IRemote);
  }

  floorNumber(num: number | undefined): string {
    if (num == null) {
      return '00:00:00';
    }

    num = Math.floor(num);

    return new Date(num * 1000).toISOString().substring(11, 19);
  }

  videoInfoDirectoryFullPath(): string {
    if (this._data.videoInfo == null) {
      return 'N/A';
    }

    return `${this._data.videoInfo.friendlyName}\\${this._data.videoInfo.file.path}\\${this._data.videoInfo.file.fileWOExt}`;
  }

  sendAskVideoInfoSetTimeOut(): void {
    if (this._data.loopAskForVideoInfo) {
      this.remoteHubService.sendAskForVideoInfo({
        profile: this._userProfile!.name,
        fromChannel: this.remoteHubService.myChannel(),
        toChannel: this._data.selectedChannel,
      } as IRemote);

      setTimeout(() => {
        this.sendAskVideoInfoSetTimeOut();
      }, 1000);
    }
  }
}
class Data {
  isConnected = false;
  otherChannels: number[] = [];
  videoInfo: IRemoteVideoInfo | null = null;
  selectedChannel = 0;
  loopAskForVideoInfo = false;
}
