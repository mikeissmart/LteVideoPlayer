import { ToasterService } from './../../../services/toaster/toaster.service';
import {
  Component,
  effect,
  inject,
  Input,
  OnInit,
  output,
  ViewChild,
} from '@angular/core';
import { ModalComponent } from '../../common/modal/modal.component';
import { DirectoryService } from '../../../services/api-services/directory.service';
import { IFile, IRemoteVideoInfo } from '../../../models/models';
import { ThumbnailService } from '../../../services/api-services/thumbnail.service';
import { MetadataModalComponent } from '../metadata-modal/metadata-modal.component';
import { RemoteHubService } from '../../../services/hubs/remote-hub.service';
import { UserProfileService } from '../../../services/api-services/user-profile.service';

@Component({
  selector: 'app-video-player-modal',
  imports: [ModalComponent, MetadataModalComponent],
  templateUrl: './video-player-modal.component.html',
  styleUrl: './video-player-modal.component.scss',
})
export class VideoPlayerModalComponent implements OnInit {
  directoryService = inject(DirectoryService);
  thumbnailService = inject(ThumbnailService);
  toasterService = inject(ToasterService);
  remoteHubService = inject(RemoteHubService);
  userProfileService = inject(UserProfileService);

  closeOutput = output({
    alias: 'close',
  });
  highlightFailedFile = output<IFile>();

  protected _player: HTMLMediaElement | null = null;
  protected _data = new Data();
  protected _isModalOpen = false;
  protected _isFirstPlay = true;

  @ViewChild('modal')
  protected _modal!: ModalComponent;
  @ViewChild('metadataModal')
  protected _metadataModal!: MetadataModalComponent;

  isOpen(): boolean {
    return this._isModalOpen;
  }

  close(): void {
    this._modal.close();
  }

  playFile(file: IFile, fileIndex: number, totalFiles: number): void {
    this._data = new Data();
    this._data.currentFile = file;
    this._data.fileIndex = fileIndex;
    this._data.totalFiles = totalFiles;
    this.directoryService.getNextFile(
      this.directoryService.currentDirectory().dir!.dirEnum,
      file,
      (result) => (this._data.nextFile = result)
    );
    if (!this._isModalOpen) {
      this._isModalOpen = true;
      this._modal.open();
    }
  }

  protected onClose(): void {
    this.clearPlayer();
    this._isModalOpen = false;
    this.closeOutput.emit();
  }

  ngOnInit(): void {
    this.remoteHubService.receiveAskForVideoInfo((data) => {
      this.sendVideoInfo(data.fromChannel);
    });
    this.remoteHubService.receiveSetSeek((data) => {
      if (this._player != null) {
        this._player.currentTime = data.seekPercent * this._player.duration;
      }
    });
    this.remoteHubService.receiveMoveSeek((data) => {
      if (this._player != null) {
        this._player.currentTime += data.seek;
      }
    });
    this.remoteHubService.receivePlayPause((data) => {
      if (this._player != null) {
        if (this._player.paused) {
          this._player.play();
        } else {
          this._player.pause();
        }
      }
    });
    this.remoteHubService.receiveSetVolume((data) => {
      if (this._player != null) {
        this._player.volume = data.volume / 100;
      }
    });
  }

  protected clearPlayer(): void {
    this._data = new Data();
    if (this._player != null) {
      this._player.pause();
      this._player.src = '';
    }
    const value = this.directoryService.getCopyCurrentDirectory();
    value.file = '';
    this.directoryService.updateCurrentDirectory(value);
  }

  protected onLoadedData(player: HTMLMediaElement): void {
    this._player = player;
    if (this._isFirstPlay) {
      this._isFirstPlay = false;
      this._player!.muted = true;
      setTimeout(() => {
        this._player!.muted = false;
        this._player!.play()
          .then(() => {
            if (
              this.remoteHubService.isConnected() &&
              this.remoteHubService.myChannel() != 0
            ) {
              this.sendVideoInfo(null);
            }
          })
          .catch((error) => {
            this.toasterService.showError(
              `Video failed to autoplay. Reselect video '${
                this._data.currentFile!.fileWOExt
              }`
            );
            this.highlightFailedFile.emit(this._data.currentFile!);
            this.close();
          });
      }, 1000);
    } else {
      this._player!.play();
    }
    this._data.isDataLoaded = true;
    this.checkThumbnail();
  }

  protected onPlayerEnded(): void {
    if (this._data.nextFile != null) {
      const value = this.directoryService.getCopyCurrentDirectory();
      value.file = this._data.nextFile.file;
      this.directoryService.updateCurrentDirectory(value);
    }
  }

  protected onKeyDown(e: KeyboardEvent): void {
    if (this._data.isDataLoaded) {
      if (e.key == 'ArrowRight') {
        this.onSkip(5);
        e.preventDefault();
      } else if (e.key == 'ArrowLeft') {
        this.onSkip(-5);
        e.preventDefault();
      }
    }
  }

  protected onSkip(seek: number): void {
    if (this._player != null) {
      this._player.currentTime += seek;
    }
  }

  protected onRestartPlayer(): void {
    if (this._player != null) {
      this._player.pause();
      this._player.currentTime = 0;
      this._player.play();
    }
  }

  protected onDeleteThumbnail(): void {
    this.thumbnailService.deleteThumbnail(
      this.directoryService.currentDirectory().dir!.dirEnum,
      this._data.currentFile!.fullPath,
      () => (this._data.hasThumbnail = false)
    );
  }

  protected onMetadata(): void {
    this._data.metadataFile = this._data.currentFile;
    this._metadataModal.open();
  }

  protected checkThumbnail(): void {
    this.thumbnailService.hasFileThumbnail(
      this.directoryService.currentDirectory().dir!.dirEnum,
      this._data.currentFile!.fullPath,
      (result) => (this._data.hasThumbnail = result)
    );
  }

  protected sendVideoInfo(toChannel: number | null): void {
    this.remoteHubService.sendVideoInfo({
      profile: this.userProfileService.currentUserProfile()!.name,
      fromChannel: this.remoteHubService.myChannel(),
      toChannel: toChannel,
      friendlyName: this.directoryService.currentDirectory().dir!.friendlyName,
      path: this._data.currentFile!.path,
      file: this._data.currentFile!,
      currentTimeSeconds: this._player!.currentTime,
      maxTimeSeconds: this._player!.duration,
      volume: this._player!.muted ? 0 : this._player!.volume * 100,
      isPlaying: !this._player!.paused,
    } as IRemoteVideoInfo);
  }
}
class Data {
  currentFile: IFile | null = null;
  nextFile: IFile | null = null;
  metadataFile: IFile | null = null;
  isDataLoaded = false;
  hasThumbnail = false;
  fileIndex = 0;
  totalFiles = 0;
}
