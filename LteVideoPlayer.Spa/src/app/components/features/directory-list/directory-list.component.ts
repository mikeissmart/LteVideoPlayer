import { ConvertFileService } from './../../../services/api-services/convert-file.service';
import {
  Component,
  computed,
  effect,
  inject,
  output,
  ViewChild,
} from '@angular/core';
import { DirectoryService } from '../../../services/api-services/directory.service';
import {
  IConvertFile,
  IDir,
  IDirsAndFiles,
  IFile,
} from '../../../models/models';
import { DirPathFile } from '../../../models/view-models';
import { ThumbnailService } from '../../../services/api-services/thumbnail.service';
import { CommonModule } from '@angular/common';
import { VideoPlayerModalComponent } from '../video-player-modal/video-player-modal.component';
import { ConvertFileAddModalComponent } from '../convert-file-add-modal/convert-file-add-modal.component';
import { ConvertDirectoryAddModalComponent } from '../convert-directory-add-modal/convert-directory-add-modal.component';
import { ModalComponent } from '../../common/modal/modal.component';
import { ConvertFileListComponent } from '../convert-file-list/convert-file-list.component';
import { ThumbnailErrorListModalComponent } from '../thumbnail-error-list-modal/thumbnail-error-list-modal.component';
import { InfiniteScrollComponent } from '../../common/infinite-scroll/infinite-scroll.component';
import { FormsModule } from '@angular/forms';
import { ToasterService } from '../../../services/toaster/toaster.service';
import { RemoteModalComponent } from '../remote-modal/remote-modal.component';
import { RemoteHubService } from '../../../services/hubs/remote-hub.service';

@Component({
  selector: 'app-directory-list',
  imports: [
    CommonModule,
    VideoPlayerModalComponent,
    ConvertFileAddModalComponent,
    ConvertDirectoryAddModalComponent,
    ModalComponent,
    ConvertFileListComponent,
    ThumbnailErrorListModalComponent,
    FormsModule,
    RemoteModalComponent,
  ],
  templateUrl: './directory-list.component.html',
  styleUrl: './directory-list.component.scss',
})
export class DirectoryListComponent {
  directoryService = inject(DirectoryService);
  thumbnailService = inject(ThumbnailService);
  convertFileService = inject(ConvertFileService);
  toasterService = inject(ToasterService);
  remoteHubService = inject(RemoteHubService);

  dirsAndFiles: IDirsAndFiles | null = null;
  displayedFiles: IFile[] = [];
  filesPerPage = 20;
  displayPage = 0;
  curDirPathFile = new DirPathFile();
  selectedConvertFileIndex = -1;
  convertFiles: IConvertFile[] = [];
  highlightFailedFile: IFile | null = null;
  searchStr = '';

  @ViewChild('videoPlayerModal')
  videoPlayerModal!: VideoPlayerModalComponent;
  @ViewChild('convertFileAddModel')
  convertFileAddModel!: ConvertFileAddModalComponent;
  @ViewChild('convertDirectoryAddModel')
  convertDirectoryAddModel!: ConvertDirectoryAddModalComponent;
  @ViewChild('convertFileListModal')
  convertFileListModal!: ModalComponent;
  @ViewChild('thumbnailErrorListModel')
  thumbnailErrorListModel!: ThumbnailErrorListModalComponent;
  @ViewChild('remoteModal')
  remoteModal!: RemoteModalComponent;

  constructor() {
    effect(() => {
      this.curDirPathFile = this.directoryService.currentDirectory();
      this.fetchDirsAndFiles();
    });
  }

  fetchDirsAndFiles(): void {
    if (this.curDirPathFile.dir != null) {
      this.directoryService.getDirsAndFiles(
        this.curDirPathFile.dir!.dirEnum,
        this.curDirPathFile.path,
        (result) => {
          this.dirsAndFiles = result;
          this.displayedFiles = [];
          this.displayPage = 0;
          this.onNearEnd();
          if (this.curDirPathFile.file.length > 0) {
            const file = result.files.find(
              (x) => x.file == this.curDirPathFile.file
            );
            if (file != null) {
              this.highlightFailedFile = null;
              this.videoPlayerModal.playFile(
                file,
                result.files.findIndex((x) => x.file == file.file),
                result.files.length
              );
            } else {
              const value = this.directoryService.getCopyCurrentDirectory();
              value.file = '';
              this.directoryService.updateCurrentDirectory(value);
            }
          } else if (this.videoPlayerModal.isOpen()) {
            this.videoPlayerModal.close();
          }
        },
        () => {
          this.toasterService.showError(
            `Unable to find ${this.curDirPathFile.path}`
          );
          const value = this.directoryService.getCopyCurrentDirectory();
          value.path = '';
          value.file = '';
          this.directoryService.updateCurrentDirectory(value);
        }
      );
    }
  }

  onNearEnd(): void {
    this.displayPage++;
    var fileIndex = this.displayPage * this.filesPerPage;
    for (
      var i = 0;
      i < this.filesPerPage && fileIndex < this.dirsAndFiles!.files.length;
      i++, fileIndex++
    ) {
      this.displayedFiles.push(this.dirsAndFiles!.files[fileIndex]);
    }
  }

  isAnyFilesQueued(): boolean {
    return this.dirsAndFiles!.files.filter((x) => x.isConvertQueued).length > 0;
  }

  pushDir(dir: IDir): void {
    const value = this.directoryService.getCopyCurrentDirectory();
    value.path = dir.fullPath;
    value.file = '';
    this.directoryService.updateCurrentDirectory(value);
  }

  pushFile(file: IFile): void {
    const value = this.directoryService.getCopyCurrentDirectory();
    value.file = file.file;
    this.directoryService.updateCurrentDirectory(value);
  }

  popDir(): void {
    const value = this.directoryService.getCopyCurrentDirectory();
    const parts = value.path.split('\\');
    var dir = '';
    for (let i = 0; i < parts.length - 1; i++) {
      dir += (dir.length > 0 ? '\\' : '') + parts[i];
    }

    value.path = dir;
    value.file = '';
    this.directoryService.updateCurrentDirectory(value);
  }

  onHighlightFailedFile(value: IFile): void {
    this.highlightFailedFile = value;
  }

  isHighlightFailedFile(value: IFile): boolean {
    return this.highlightFailedFile?.file == value.file;
  }

  showAllConvertList(): void {
    this.convertFileService.getAllConvertFiles((result) => {
      this.convertFiles = result;
      this.convertFileListModal.open();
    });
  }

  showFileConvertList(file: IFile): void {
    this.convertFileService.getConvertFileByOriginalFile(
      this.curDirPathFile.dir!.dirEnum,
      file,
      (result) => {
        this.convertFiles = result;
        this.convertFileListModal.open();
      }
    );
  }

  convertFile(file: IFile, index: number): void {
    this.selectedConvertFileIndex = index;
    this.convertFileAddModel.setOriginalFile(file);
    this.convertFileAddModel.open();
  }

  convertDirectory(): void {
    this.convertDirectoryAddModel.setOritinalFiles(this.dirsAndFiles!.files);
    this.convertDirectoryAddModel.open();
  }

  onFileConvertQueued(file: IFile): void {
    this.dirsAndFiles!.files[this.selectedConvertFileIndex] = file;
    this.convertFileAddModel.close();
  }

  onDirectoryConvertQueued(files: IFile[]): void {
    this.dirsAndFiles!.files = files;
    this.convertDirectoryAddModel.close();
  }

  showAllThumbnailErrorsList(): void {
    this.thumbnailErrorListModel.open();
  }
}
