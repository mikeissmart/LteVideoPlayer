import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-video-file-select',
  templateUrl: './video-file-select.component.html',
  styleUrls: ['./video-file-select.component.scss'],
})
export class VideoFileSelectComponent implements OnInit {
  ngOnInit(): void {}
  /*dirs: string[] = [];
  dirLevel = 0;
  selectedDir = '';
  videoFiles: IVideoFileDto[] = [];
  search: string[] = ['', ''];

  @Output()
  onPlayVideoFile = new EventEmitter<IVideoFileDto>();

  constructor(private readonly videoFileService: VideoFileService) {}

  ngOnInit(): void {
    this.getAllDirsAndFiles();
  }

  getAllDirsAndFiles(): void {
    this.videoFileService.getAllVideoDirsAndFiles((result) => {
      this.dirs = result.dirs ?? [];
      this.videoFiles = result.videoFiles ?? [];
    });
  }

  getSearchedDirs(): string[] {
    const dirsAtLevel = [] as string[];
    this.dirs.forEach((x) => {
      if (x.includes(this.selectedDir)) {
        const xSplit = x.split('\\');
        if (xSplit.length > this.dirLevel) {
          const dirPart = xSplit[this.dirLevel];
          if (
            dirPart != '' &&
            !dirsAtLevel.includes(dirPart) &&
            dirPart
              .toLocaleLowerCase()
              .includes(this.search[this.dirLevel].toLocaleLowerCase())
          ) {
            dirsAtLevel.push(dirPart);
          }
        }
      }
    });

    return dirsAtLevel;
  }

  getDirFiles(): IVideoFileDto[] {
    const videos = this.videoFiles.filter(
      (x) => x.videoPath! == this.selectedDir
    );
    return videos;
  }

  popDir(): void {
    const split = this.selectedDir.split('\\');
    this.selectedDir = this.selectedDir.slice(
      0,
      this.selectedDir.indexOf(split[split.length - 2])
    );
    this.dirLevel--;
  }

  selectDir(dir: string) {
    this.dirLevel++;
    this.selectedDir += `${dir}\\`;
  }*/
}
