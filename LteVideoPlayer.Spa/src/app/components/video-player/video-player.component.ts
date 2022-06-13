import { Component, Input, OnInit } from '@angular/core';
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

  constructor(public directoryService: DirectoryService) {}

  ngOnInit(): void {}

  playFile(file: IFileDto, isStaging: boolean): void {
    this.currentFile = file;
    this.isStaging = isStaging;
    this.directoryService.getNextFile(
      file,
      isStaging,
      (result) => (this.nextFile = result),
      (error) => (this.errors = error)
    );
  }

  onLoadedData(player: HTMLMediaElement): void {
    this.player = player;
    this.player.play();
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
