/*import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IDirDto, IFileDto } from 'src/app/models/models';

@Component({
  selector: 'app-video-select',
  templateUrl: './video-select.component.html',
  styleUrls: ['./video-select.component.scss'],
})
export class VideoSelectComponent implements OnInit {
  dirDto: IDirDto = {
    path: '',
    name: '',
    videos: [],
    subDirs: [],
  };
  selectedDirs: IDirDto[] = [];
  search = '';
  queuedFiles: IFileDto[] = [];
  currentFile: IFileDto | null = null;

  isPlaying = false;
  isExpanded = true;
  player: HTMLMediaElement | null = null;

  constructor(private readonly httpClient: HttpClient) {}

  ngOnInit(): void {
    this.selectedDirs.push(this.dirDto);

    this.httpClient
      .get<IDirDto[]>(
        'https://localhost:7183/api/videoplayer/GetSeriesAndSeasons'
      )
      .subscribe({
        next: (result) => {
          this.dirDto.subDirs = result;
          this.selectedDirs.pop();
          this.selectedDirs.push(this.dirDto);
        },
      });
  }

  convertFileToMp4(sourceFile: IFileDto): void {
    this.pausePlayer();

    this.httpClient
      .post<IFileDto | null>(
        'https://localhost:7183/api/videoplayer/ConvertFile',
        sourceFile,
        {
          headers: new HttpHeaders({
            'Content-Type': 'application/json',
          }),
        }
      )
      .subscribe({
        next: (result) => {
          if (result == null) {
            alert('Failed to convert file');
          } else {
            this.queuedFiles[0] = result;
            this.currentFile = result;
          }
        },
      });
  }

  pushDir(dir: IDirDto): void {
    this.selectedDirs.push(dir);
  }

  popDir(): void {
    this.selectedDirs.pop();
  }

  currentDir(): IDirDto {
    return this.selectedDirs[this.selectedDirs.length - 1];
  }

  currentSubDirs(): IDirDto[] {
    const curDir = this.currentDir();

    if (this.isCurrentRootDir()) {
      return curDir.subDirs!.filter((x) =>
        x.name!.toLocaleLowerCase().includes(this.search.toLocaleLowerCase())
      );
    }

    return curDir.subDirs!;
  }

  isCurrentRootDir(): boolean {
    return this.selectedDirs.length == 1;
  }

  nextQueuedFile(): IFileDto | null {
    if (this.queuedFiles.length > 1) {
      return this.queuedFiles[1];
    } else {
      return null;
    }
  }

  playFile(file: IFileDto): void {
    this.pausePlayer();

    this.queuedFiles = [];
    this.queuedFiles.push(file);
    this.currentFile = file;
    this.isExpanded = false;
  }

  playlist(file: IFileDto): void {
    const curDir = this.currentDir();
    this.pausePlayer();

    const startIndex = curDir.videos!.findIndex((x) => x.name == file.name);
    this.queuedFiles = [];
    for (let i = startIndex; i < curDir.videos!.length; i++) {
      this.queuedFiles.push(curDir.videos![i]);
    }

    this.currentFile = this.queuedFiles[0];
    this.isExpanded = false;
  }

  onLoadedData(player: HTMLMediaElement): void {
    this.player = player;
    this.player.play();
  }

  getCurrentFileApiUrl(): string | null {
    if (this.currentFile == null) {
      return null;
    } else {
      return (
        'https://localhost:7183/api/videoplayer/StreamVideo?fileDir=' +
        this.currentFile.path
      );
    }
  }

  pausePlayer(): void {
    if (this.isPlaying) {
      this.player?.pause();
    }
  }

  getExt(file: string): string {
    const split = file.split('.');

    return split[split.length - 1];
  }

  onPlaying(): void {
    this.isPlaying = true;
    this.isExpanded = false;
  }

  onPlayerEnded(): void {
    this.queuedFiles.pop();
  }
}*/
