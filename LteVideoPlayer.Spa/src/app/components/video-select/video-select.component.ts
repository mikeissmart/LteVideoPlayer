import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IDirDto } from 'src/app/models/models';

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
  queueFiles: string[] = [];
  sourceFiles: any[] = [];
  sourceFileIndex = 0;
  currentSourceFile: any | null = null;
  isPlaying = false;

  @ViewChild('player', { static: false })
  player: any;

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

  playFile(file: string): void {
    const curDir = this.currentDir();
    const playFiles = [];
    playFiles.push(curDir.path + '\\' + curDir.name + '\\' + file);

    this.queueFiles = [];
    this.queueFiles.push(file);
    this.convertToVideoSource(playFiles);
  }

  playlist(file: string): void {
    const curDir = this.currentDir();
    const startIndex = curDir.videos!.findIndex((x) => x == file);

    this.queueFiles = [];
    const playFiles = [];
    for (let i = startIndex; i < curDir.videos!.length; i++) {
      this.queueFiles.push(curDir.videos![i]);
      playFiles.push(
        curDir.path + '\\' + curDir.name + '\\' + curDir.videos![i]
      );
    }

    this.convertToVideoSource(playFiles);
  }

  convertToVideoSource(playFiles: string[]): any {
    this.sourceFiles = [];
    for (let i = 0; i < playFiles.length; i++) {
      const file = playFiles[i];
      this.sourceFiles.push(
        'https://localhost:7183/api/videoplayer/StreamVideo?fileDir=' + file
      );
    }

    if (this.isPlaying) {
      this.player.pause();
    }
    this.sourceFileIndex = 0;
    this.currentSourceFile = this.sourceFiles[0];
  }

  getExt(file: string): string {
    const split = file.split('.');

    return split[split.length - 1];
  }

  onPlayerEnded(): void {
    this.sourceFileIndex++;

    if (this.sourceFileIndex === this.sourceFiles.length) {
      this.currentSourceFile = null;
    }

    this.currentSourceFile = this.sourceFiles[this.sourceFileIndex];
  }
}
