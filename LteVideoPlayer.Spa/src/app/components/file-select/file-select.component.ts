import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { IDirDto, IDirsAndFilesDto, IFileDto } from 'src/app/models/models';
import { DirectoryService } from 'src/app/services/api-services/directory.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';
import { ConvertFileAddManyComponent } from '../convert-file-add-many/convert-file-add-many.component';
import { ConvertFileAddComponent } from '../convert-file-add/convert-file-add.component';
import { ConvertFileListAllComponent } from '../convert-file-list-all/convert-file-list-all.component';
import { ModalComponent } from '../modal/modal.component';
import { VideoPlayerComponent } from '../video-player/video-player.component';

@Component({
  selector: 'app-file-select',
  templateUrl: './file-select.component.html',
  styleUrls: ['./file-select.component.scss'],
})
export class FileSelectComponent implements OnInit {
  errors: ModelStateErrors | null = null;
  selectedDirs: IDirDto[] = [
    {
      dirPath: '',
      dirName: '',
      dirPathName: '',
    } as IDirDto,
  ];
  dirsAndFiles: IDirsAndFilesDto = {
    dirs: [],
    files: [],
  } as IDirsAndFilesDto;
  isStaging = false;

  public get currentDir(): IDirDto {
    return this.selectedDirs[this.selectedDirs.length - 1];
  }

  @ViewChild('convertAllModal')
  convertAllModal: ModalComponent | null = null;
  @ViewChild('convertAll')
  convertAll: ConvertFileAddManyComponent | null = null;

  @ViewChild('convertModal')
  convertModal: ModalComponent | null = null;
  @ViewChild('convert')
  convert: ConvertFileAddComponent | null = null;

  @ViewChild('convertListAllModal')
  convertListAllModal: ModalComponent | null = null;
  @ViewChild('convertListAll')
  convertListAll: ConvertFileListAllComponent | null = null;

  @ViewChild('videoPlayerModal')
  videoPlayerModal: ModalComponent | null = null;
  @ViewChild('videoPlayer')
  videoPlayer: VideoPlayerComponent | null = null;

  @Input()
  isAdmin = false;

  constructor(private readonly directoryService: DirectoryService) {}

  ngOnInit(): void {
    this.fetchDirsAndFiles();
  }

  stagingChanged(): void {
    this.isStaging = !this.isStaging;
    while (this.selectedDirs.length != 1) {
      this.selectedDirs.pop();
    }
    this.fetchDirsAndFiles();
  }

  pushDir(dir: IDirDto): void {
    this.selectedDirs.push(dir);
    this.fetchDirsAndFiles();
  }

  popDir(): void {
    this.selectedDirs.pop();
    this.fetchDirsAndFiles();
  }

  fetchDirsAndFiles(): void {
    this.directoryService.getDirsAndFiles(
      this.currentDir.dirPathName!,
      this.isStaging,
      (result) => {
        this.errors = null;
        this.dirsAndFiles = result;
      },
      (error) => {
        this.errors = error;
        const reset = [];
        reset.push(this.selectedDirs[0]);
        this.selectedDirs = reset;
        this.fetchDirsAndFiles();
      }
    );
  }

  convertFile(file: IFileDto): void {
    this.convert!.setOriginalFile(file);
    this.convertModal!.openModal();
  }

  setConvertQueued(fileName: string): void {
    this.dirsAndFiles.files!.forEach((x) => {
      if (x.fileName! == fileName) {
        x.convertQueued = true;
      }
    });
  }

  setConvertQueuedAndCloseModal(fileName: string): void {
    this.dirsAndFiles.files!.forEach((x) => {
      if (x.fileName! == fileName) {
        x.convertQueued = true;
      }
    });
    this.convertModal?.closeModal();
  }

  convertAllFiles(): void {
    this.convertAll!.setOriginals(this.currentDir, this.dirsAndFiles.files!);
    this.convertAllModal!.openModal();
  }

  convertFilesListAll(): void {
    this.convertListAll!.refreshConvertFiles();
    this.convertListAllModal!.openModal();
  }

  playFile(file: IFileDto): void {
    this.videoPlayer?.playFile(file, this.isStaging);
    this.videoPlayerModal?.openModal();
  }
}
