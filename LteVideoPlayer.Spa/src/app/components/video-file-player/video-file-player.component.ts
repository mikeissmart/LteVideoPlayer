import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-video-file-player',
  templateUrl: './video-file-player.component.html',
  styleUrls: ['./video-file-player.component.scss'],
})
export class VideoFilePlayerComponent implements OnInit {
  ngOnInit(): void {}

  /*currentVideo: IVideoFileDto | null = null;
  currentErrors: ModelStateErrors | null = null;
  player: HTMLMediaElement | null = null;
  errors: ModelStateErrors | null = null;

  @Output()
  onPlayingStateChange = new EventEmitter<boolean>();

  constructor(
    public videoFileService: VideoFileService,
    private readonly convertVideoFileService: ConvertVideoFileService,
    private readonly userProfileService: UserProfileService,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {}

  playVideo(videoFileId: number): void {
    this.videoFileService.getVideoFileById(
      videoFileId,
      (result) => {
        this.currentVideo = result;
        this.onPlayingStateChange.emit(true);
      },
      (error) => (this.currentErrors = error)
    );
  }

  onLoadedData(player: HTMLMediaElement): void {
    this.player = player;
    this.player.play();
  }

  playing(): void {}

  playerEnded(): void {
    if (this.currentVideo!.nextVideoFileId != null) {
      this.playVideo(this.currentVideo!.nextVideoFileId);
    } else {
      this.currentVideo = null;
      this.onPlayingStateChange.emit(false);
    }
  }

  convertFileToMp4(): void {
    this.convertVideoFileService.addConvert(
      this.userProfileService.getCurrentUserProfile()!.id!,
      this.currentVideo!.id!,
      (result) => {},
      (error) => (this.errors = error)
    );
  }*/
}
