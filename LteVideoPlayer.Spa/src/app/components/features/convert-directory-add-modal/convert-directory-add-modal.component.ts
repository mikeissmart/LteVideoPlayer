import { Component, inject, output, ViewChild } from '@angular/core';
import { ModalComponent } from '../../common/modal/modal.component';
import { ConvertFileService } from '../../../services/api-services/convert-file.service';
import { DirectoryService } from '../../../services/api-services/directory.service';
import {
  ICreateDirectoryConvert,
  ICreateFileConvert,
  IFile,
} from '../../../models/models';
import { ModelStateErrors } from '../../../models/ModelStateErrors';
import { InputValidationComponent } from '../../common/input-validation/input-validation.component';
import { FormsModule } from '@angular/forms';
import { MetadataModalComponent } from '../metadata-modal/metadata-modal.component';

@Component({
  selector: 'app-convert-directory-add-modal',
  imports: [
    ModalComponent,
    InputValidationComponent,
    FormsModule,
    MetadataModalComponent,
  ],
  templateUrl: './convert-directory-add-modal.component.html',
  styleUrl: './convert-directory-add-modal.component.scss',
})
export class ConvertDirectoryAddModalComponent {
  directoryService = inject(DirectoryService);
  convertFileService = inject(ConvertFileService);

  directoryConvertQueued = output<IFile[]>();
  closeOutput = output({
    alias: 'close',
  });

  @ViewChild('modal')
  protected _modal!: ModalComponent;
  @ViewChild('metadataModal')
  protected _metadataModal!: MetadataModalComponent;

  protected _data = new Data();

  open(): void {
    this._modal.open();
  }

  close(): void {
    this._modal.close();
  }

  protected onClose(): void {
    this.closeOutput.emit();
  }

  setOritinalFiles(value: IFile[]): void {
    this._data = new Data();
    this._data.files = value;
    this._data.path = this.directoryService.currentDirectory().path;

    value.forEach((x) => {
      this._data.convertQueues.push({
        file: x,
        suffix: '',
        skip: false,
      });
    });
  }

  protected displayMetaInfo(): void {
    this._data.metadataFile = this._data.files[0];
    this._metadataModal.open();
  }

  protected getEpisodeNumber(queue: ConvertQueue): string {
    if (queue.skip) {
      return 'N/A';
    }

    const index = this._data.convertQueues.indexOf(queue);
    var number = index + 1;
    for (var i = 0; i < index + 1; i++) {
      if (this._data.convertQueues[i].skip) {
        number--;
      }
    }

    const leftMaxPad = `${this._data.files.length}`.length;
    var numStr = `${number}`;
    while (numStr.length != leftMaxPad) {
      numStr = `0${numStr}`;
    }

    return numStr;
  }

  protected onSkipFileToggle(queue: ConvertQueue): void {
    queue.skip = !queue.skip;
  }

  protected onMpveUp(queue: ConvertQueue, index: number): void {
    const temp = this._data.convertQueues[index - 1];
    this._data.convertQueues[index - 1] = queue;
    this._data.convertQueues[index] = temp;
  }

  protected onMoveDown(queue: ConvertQueue, index: number): void {
    const temp = this._data.convertQueues[index + 1];
    this._data.convertQueues[index + 1] = queue;
    this._data.convertQueues[index] = temp;
  }

  protected onSave(): void {
    if (this.isValid()) {
      const converts = [] as ICreateFileConvert[];
      this._data.convertQueues
        .filter((x) => !x.skip)
        .forEach((x) => {
          converts.push({
            directoryEnum:
              this.directoryService.currentDirectory().dir!.dirEnum,
            originalFile: x.file,
            convertedFile: {
              path: this._data.path,
              file: `Episode ${this.getEpisodeNumber(x)}${
                x.suffix.length > 0 ? ' ' + x.suffix : ''
              }.mp4`,
            } as IFile,
            audioStreamNumber: this._data.audioStreamNumber,
          } as ICreateFileConvert);
        });

      this.convertFileService.addConvertDirectory(
        this.directoryService.currentDirectory().dir!.dirEnum,
        {
          converts: converts,
        } as ICreateDirectoryConvert,
        (result) => {
          this._data.files.forEach((x) => (x.isConvertQueued = true));
          this.directoryConvertQueued.emit(this._data.files);
        },
        (errors) => (this._data.errors = errors)
      );
    }
  }

  protected isValid(): boolean {
    var errors = new ModelStateErrors();

    if (this._data.audioStreamNumber < 1) {
      errors.addPropertyError('audioIndex', 'AudioIndex must be >= 1');
    }

    if (errors.errors.length > 0) {
      this._data.errors = errors;
    } else {
      this._data.errors = null;
    }

    return this._data.errors == null;
  }
}
class Data {
  files: IFile[] = [];
  path = '';
  convertQueues: ConvertQueue[] = [];
  audioStreamNumber = 1;
  errors: ModelStateErrors | null = null;
  metadataFile: IFile | null = null;
}
class ConvertQueue {
  file!: IFile;
  suffix = '';
  skip = false;
}
