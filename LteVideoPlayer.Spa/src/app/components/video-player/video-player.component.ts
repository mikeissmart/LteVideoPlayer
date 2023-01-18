import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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

  @Output()
  onFilePlayChange = new EventEmitter<IFileDto>();

  constructor(public directoryService: DirectoryService) {}

  ngOnInit(): void {}

  playFile(file: IFileDto, isStaging: boolean): void {
    this.currentFile = file;
    this.onFilePlayChange.emit(file);
    this.isStaging = isStaging;
    this.player?.play();
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
      setTimeout(() => {
        this.player!.muted = false;
        console.log('timer');
      }, 1000);
    } else {
      this.player.muted = false;
    }
    this.player.play();
    //this.isPlaying = true;
  }

  playerEnded(): void {
    if (this.nextFile != null) {
      this.playFile(this.nextFile!, this.isStaging);
    }
  }

  restartPlayer(): void {
    this.player?.pause();
    this.player!.currentTime = 0;
    this.player?.play();
  }
}
