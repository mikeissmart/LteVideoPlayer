import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { IDirDto, IDirsAndFilesDto, IFileDto } from 'src/app/models/models';
import { DirectoryService } from 'src/app/services/api-services/directory.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';
import { ConvertFileAddManyComponent } from '../convert-file-add-many/convert-file-add-many.component';
import { ConvertFileAddComponent } from '../convert-file-add/convert-file-add.component';
import { ConvertFileListAllComponent } from '../convert-file-list-all/convert-file-list-all.component';
import { ModalComponent } from '../modal/modal.component';
import { VideoPlayerComponent } from '../video-player/video-player.component';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-file-select',
  templateUrl: './file-select.component.html',
  styleUrls: ['./file-select.component.scss'],
})
export class FileSelectComponent implements OnInit {
  errors: ModelStateErrors | null = null;
  dirsAndFiles: IDirsAndFilesDto = {
    dirs: [],
    files: [],
  } as IDirsAndFilesDto;
  isStaging = false;
  isFirstChange = true;

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

  @Input()
  currentDirPathName = '';

  @Input()
  currentFileName: string | null = null;

  @Output()
  onDirOrFileChange = new EventEmitter<string | null>();

  constructor(
    private readonly directoryService: DirectoryService,
    private readonly titleService: Title
  ) {}

  ngOnInit(): void {
    this.fetchDirsAndFiles();
    this.updateTitle();
  }

  stagingChanged(): void {
    this.isStaging = !this.isStaging;
    this.currentDirPathName = '';
    this.fetchDirsAndFiles();
    this.updateTitle();
  }

  pushDir(dir: IDirDto): void {
    this.currentDirPathName = dir.dirPathName!;
    this.updateTitle();
    this.onDirOrFileChange.emit(null);
    this.fetchDirsAndFiles();
  }

  popDir(): void {
    const dirs = this.currentDirPathName.split('\\');
    this.currentDirPathName = '';
    for (let index = 0; index < dirs.length - 2; index++) {
      this.currentDirPathName += dirs[index] + '\\';
    }
    this.updateTitle();
    this.onDirOrFileChange.emit(null);
    this.fetchDirsAndFiles();
  }

  routeChangeFetchDirAndFiles(dir: string, file: string | null): void {
    if (dir != this.currentDirPathName || file != this.currentFileName) {
      this.currentDirPathName = dir;
      //this.currentFileName = file;

      this.directoryService.getDirsAndFiles(
        this.currentDirPathName,
        this.isStaging,
        (result) => {
          this.errors = null;
          this.dirsAndFiles = result;
          if (
            file != null &&
            file != this.currentFileName &&
            result.files != null &&
            result.files!.length > 0
          ) {
            const resutlFiles = result.files!.filter((x) => x.fileName == file);
            if (resutlFiles.length > 0) {
              this.playFile(resutlFiles[0]);
            }
          } else {
            this.videoPlayerModal?.closeModal();
          }
        },
        (error) => {
          this.errors = error;
          this.currentDirPathName = '';
          this.fetchDirsAndFiles();
        }
      );
    }
  }

  fetchDirsAndFiles(): void {
    this.directoryService.getDirsAndFiles(
      this.currentDirPathName,
      this.isStaging,
      (result) => {
        this.errors = null;
        this.dirsAndFiles = result;

        if (
          this.currentFileName != null &&
          result.files != null &&
          result.files!.length > 0
        ) {
          const resutlFiles = result.files!.filter(
            (x) => x.fileName == this.currentFileName
          );
          if (resutlFiles.length > 0) {
            this.playFile(resutlFiles[0]);
          }
        }

        /*if (playFile && result.files != null && result.files!.length > 0) {
          const file = result.files!.filter(
            (x) => x.fileName == this.currentFileName
          );
          if (file.length > 0 && file[0].fileName != this.currentFileName) {
            this.playFile(file[0]);
          }
        } else {
          this.videoPlayerModal?.closeModal();
        }*/
      },
      (error) => {
        this.errors = error;
        this.currentDirPathName = '';
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
    this.convertAll!.setOriginals(
      this.currentDirPathName,
      this.dirsAndFiles.files!
    );
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

  onPlayFileChange(file: IFileDto): void {
    this.currentFileName = file.fileName!;
    this.currentDirPathName = file.filePath!;
    this.onDirOrFileChange.emit(file.fileName!);
  }

  onPlayerClose(): void {
    this.updateTitle();
    this.currentFileName = null;
    this.onDirOrFileChange.emit(null);
  }

  updateTitle() {
    if (this.currentDirPathName.length > 0) {
      this.titleService.setTitle(this.currentDirPathName);
    } else {
      this.titleService.setTitle('LteVideoPlayer');
    }
  }
}
