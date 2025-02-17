import {
  Component,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { Title } from '@angular/platform-browser';
import { IFileDto, IRemoteData_VideoInfoDto } from 'src/app/models/models';
import { DirectoryService } from 'src/app/services/api-services/directory.service';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';
import { RemoteHubService } from 'src/app/services/hubs/remote-hub.service';
import { ModalComponent } from '../modal/modal.component';
import { ThumbnailService } from 'src/app/services/api-services/thumbnail.service';

@Component({
  selector: 'app-video-player',
  templateUrl: './video-player.component.html',
  styleUrls: ['./video-player.component.scss'],
})
export class VideoPlayerComponent implements OnInit {
  errors: ModelStateErrors | null = null;
  currentFile: IFileDto | null = null;
  nextFile: IFileDto | null = null;
  isStaging = false;
  player: HTMLMediaElement | null = null;
  isFirstPlay = true;
  isDataLoaded = false;
  isModalOpen = false;
  hasThumbnail = false;
  myChannel = 0;

  @ViewChild('videoPlayerModal')
  videoPlayerModal: ModalComponent | null = null;

  @Output()
  onFilePlayChange = new EventEmitter<IFileDto>();
  @Output()
  onPlayerClose = new EventEmitter();
  @Output()
  onVideoMeta = new EventEmitter<IFileDto>();

  constructor(
    public directoryService: DirectoryService,
    public thumbnailService: ThumbnailService,
    private readonly remoteHubService: RemoteHubService,
    private readonly userProfileService: UserProfileService,
    private readonly titleService: Title
  ) {}

  ngOnInit(): void {
    this.remoteHubService.receiveYourChannel(
      (channel) => (this.myChannel = channel)
    );

    this.remoteHubService.receiveAskForVideoInfo((data) =>
      this.sendVideoInfo()
    );

    this.remoteHubService.receiveSetSeek((data) => {
      if (this.player != null && this.myChannel == data.channel) {
        this.player.currentTime =
          data.seekPercentPosition! * this.player.duration;
        this.sendVideoInfo();
      }
    });

    this.remoteHubService.receiveMoveSeek((data) => {
      if (this.player != null && this.myChannel == data.channel) {
        this.player.currentTime += data.seekPosition!;
        this.sendVideoInfo();
      }
    });

    this.remoteHubService.receiveVideoPause((data) => {
      if (this.player != null && this.myChannel == data.channel) {
        this.player.pause();
        this.sendVideoInfo();
      }
    });

    this.remoteHubService.receiveVideoPlay((data) => {
      if (this.player != null && this.myChannel == data.channel) {
        this.player.play();
        this.sendVideoInfo();
      }
    });

    this.remoteHubService.receiveSetVolume((data) => {
      if (this.player != null && this.myChannel == data.channel) {
        if (data.volume != 0 && this.player.muted) {
          this.player.muted = false;
        }
        this.player.volume = data.volume! / 100;
        this.sendVideoInfo();
      }
    });

    this.remoteHubService.receiveVideoFullScreen((data) => {
      if (this.player != null && this.myChannel == data.channel) {
        this.player.requestFullscreen({
          navigationUI: 'show',
        } as FullscreenOptions);
        this.sendVideoInfo();
      }
    });
  }

  clearPlayer(): void {
    this.currentFile = null;
    this.nextFile = null;
    this.isFirstPlay = false;
    this.isModalOpen = false;
    if (this.player != null) {
      this.player!.src = '';
    }
    this.onPlayerClose.emit();
  }

  playFile(file: IFileDto, isStaging: boolean): void {
    this.currentFile = file;
    this.titleService.setTitle(file.filePathName!);
    this.onFilePlayChange.emit(file);
    this.isStaging = isStaging;
    this.isDataLoaded = false;
    this.directoryService.getNextFile(
      file,
      isStaging,
      (result) => (this.nextFile = result),
      (error) => (this.errors = error)
    );
    if (!this.isModalOpen) {
      this.isModalOpen = true;
      this.videoPlayerModal?.openModal();
    }
  }

  closeModal(): void {
    this.clearPlayer();
    this.videoPlayerModal?.closeModal();
  }

  onLoadedData(player: HTMLMediaElement): void {
    this.player = player;
    if (this.isFirstPlay) {
      this.player.muted = true;
      this.isFirstPlay = false;
      setTimeout(() => {
        this.player!.muted = false;
        this.player!.play();
      }, 1000);
      setInterval(() => {
        this.sendVideoInfo();
      }, 30 * 1000);
    } else {
      this.player.play();
      this.sendVideoInfo();
    }
    this.isDataLoaded = true;
    this.checkThumbnail();
  }

  playerEnded(): void {
    if (this.nextFile != null) {
      this.playFile(this.nextFile!, this.isStaging);
    }
  }

  skip(time: number): void {
    if (this.player != null) {
      this.player.currentTime += time;
    }
  }

  restartPlayer(): void {
    this.player?.pause();
    this.player!.currentTime = 0;
    this.player?.play();
  }

  sendVideoInfo(): void {
    this.remoteHubService.sendVideoInfo({
      profile: this.userProfileService.getCurrentUserProfile()!.name!,
      channel: this.myChannel,
      videoFile: this.isModalOpen
        ? this.currentFile?.filePathName ?? 'N/A'
        : 'N/A',
      currentTimeSeconds: this.player?.currentTime,
      maxTimeSeconds: this.isModalOpen ? this.player?.duration : 0,
      volume:
        this.player != null
          ? this.player.muted
            ? 0
            : this.player?.volume * 100
          : 0,
      isPlaying: !this.player?.paused,
    } as IRemoteData_VideoInfoDto);
  }

  checkThumbnail(): void {
    this.thumbnailService.hasFileThumbnail(this.currentFile!.filePathName!,
      (exists: boolean) => {
        this.hasThumbnail = exists;
      }
    );
  }

  deleteThumbnail(): void {
    this.thumbnailService.deleteThumbnail(this.currentFile!.filePathName!, () => {
      this.checkThumbnail();
    });
  }

  displayMetaInfo(): void {
    this.onVideoMeta.emit(this.currentFile!);
  }

  onKeyDown(e: KeyboardEvent): void {
    if (this.isDataLoaded) {
      if (e.key == 'ArrowRight') {
        this.skip(5);
        e.preventDefault();
      } else if (e.key == 'ArrowLeft') {
        this.skip(-5);
        e.preventDefault();
      }
    }
  }
}
