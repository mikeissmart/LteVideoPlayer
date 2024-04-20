import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { map } from 'rxjs';
import {
  IRemoteDataDto,
  IRemoteData_FullScreenDto,
  IRemoteData_MoveSeekDto,
  IRemoteData_SetVolumeDto,
  IRemoteData_VideoInfoDto,
} from 'src/app/models/models';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { RemoteHubService } from 'src/app/services/hubs/remote-hub.service';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-remote',
  templateUrl: './remote.component.html',
  styleUrls: ['./remote.component.scss'],
})
export class RemoteComponent implements OnInit, OnDestroy {
  myChannel = 0;
  selectedChannel = 0;
  otherChannels: number[] = [];
  videoInfo: IRemoteData_VideoInfoDto | null = null;
  isConnected = false;

  @ViewChild('remoteModal')
  remoteModal: ModalComponent | null = null;

  constructor(
    private readonly remoteHubService: RemoteHubService,
    private readonly userProfileService: UserProfileService
  ) {}

  ngOnInit(): void {
    this.remoteHubService
      .observable()
      .subscribe((isConnected) => (this.isConnected = isConnected));

    this.connectRemote();

    this.remoteHubService.receiveAddChannel((channel) => {
      if (this.otherChannels.indexOf(channel) == -1) {
        this.otherChannels.push(channel);
      }
    });

    this.remoteHubService.receiveYourChannel(
      (channel) => (this.myChannel = channel)
    );

    this.remoteHubService.receiveRemoveChannel(
      (channel) =>
        (this.otherChannels = this.otherChannels.filter((x) => x != channel))
    );

    this.remoteHubService.receiveVideoInfo((videoInfo) => {
      if (videoInfo.channel == this.selectedChannel) {
        this.videoInfo = videoInfo;
      }
    });

    this.remoteHubService.receiveError((error) =>
      console.error(`RemoteComponent: ${error}`)
    );
  }

  ngOnDestroy(): void {
    this.myChannel = 0;
    this.otherChannels = [];
    this.remoteHubService.disconnect();
  }

  openModal(): void {
    this.remoteModal?.openModal();
  }

  floorNumber(num: number | undefined): string {
    if (num == null) {
      return '';
    }

    num = Math.floor(num);

    return new Date(num * 1000).toISOString().substring(11, 19);
  }

  connectRemote(): void {
    this.remoteHubService.connect(() => {
      this.remoteHubService.sendJoinProfile(
        this.userProfileService.getCurrentUserProfile()!.name!
      );
    });
  }

  reconnectRemote(): void {
    this.remoteHubService.disconnect(() => this.connectRemote());
  }

  onChannelChange(): void {
    if (this.selectedChannel == 0) {
      this.videoInfo = null;
    } else {
      this.remoteHubService.sendAskForVideoInfo({
        profile: this.userProfileService.getCurrentUserProfile()!.name!,
        channel: this.selectedChannel,
      } as IRemoteDataDto);
    }
  }

  resetChannel(): void {
    this.remoteHubService.sendResetChannel();
  }

  refreshChannels(): void {
    this.otherChannels = [];
    this.remoteHubService.sendGetChannels();
  }

  refreshVideoInfo(): void {
    this.remoteHubService.sendAskForVideoInfo({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
    } as IRemoteDataDto);
  }

  reset(): void {
    this.remoteHubService.sendSetSeek({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
      seekPercentPosition: 0,
    } as IRemoteData_VideoInfoDto);
  }

  skip(value: number): void {
    this.remoteHubService.sendMoveSeek({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
      seekPosition: value,
    } as IRemoteData_MoveSeekDto);
  }

  next(): void {
    this.remoteHubService.sendSetSeek({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
      seekPercentPosition: 1,
    } as IRemoteData_VideoInfoDto);
  }

  play(): void {
    this.remoteHubService.sendVideoPlay({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
    } as IRemoteDataDto);
  }

  pause(): void {
    this.remoteHubService.sendVideoPause({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
    } as IRemoteDataDto);
  }

  changeVolume(volume: number): void {
    this.remoteHubService.sendSetVolume({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
      volume: volume * 1.0,
    } as IRemoteData_SetVolumeDto);
  }

  fullScreen(): void {
    this.remoteHubService.sendVideoFullScreen({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.selectedChannel,
    } as IRemoteData_FullScreenDto);
  }
}
