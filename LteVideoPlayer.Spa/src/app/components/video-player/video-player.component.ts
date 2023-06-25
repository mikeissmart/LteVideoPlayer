import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { IFileDto } from 'src/app/models/models';
import { DirectoryService } from 'src/app/services/api-services/directory.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';

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

  @Output()
  onFilePlayChange = new EventEmitter<IFileDto>();

  constructor(
    public directoryService: DirectoryService,
    private readonly titleService: Title
  ) {}

  ngOnInit(): void {}

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
    } else {
      this.player.play();
    }
    this.isDataLoaded = true;
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
}
