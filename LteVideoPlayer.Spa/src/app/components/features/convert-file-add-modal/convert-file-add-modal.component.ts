import { ConvertFileService } from './../../../services/api-services/convert-file.service';
import {
  IConvertFile,
  ICreateFileConvert,
  IFile,
} from './../../../models/models.d';
import { Component, output, ViewChild, inject } from '@angular/core';
import { ModalComponent } from '../../common/modal/modal.component';
import { InputValidationComponent } from '../../common/input-validation/input-validation.component';
import { ModelStateErrors } from '../../../models/ModelStateErrors';
import { FormsModule } from '@angular/forms';
import { DirectoryService } from '../../../services/api-services/directory.service';
import { ConvertFileListComponent } from '../convert-file-list/convert-file-list.component';

@Component({
  selector: 'app-convert-file-add-modal',
  imports: [
    ModalComponent,
    InputValidationComponent,
    FormsModule,
    ConvertFileListComponent,
  ],
  templateUrl: './convert-file-add-modal.component.html',
  styleUrl: './convert-file-add-modal.component.scss',
})
export class ConvertFileAddModalComponent {
  directoryService = inject(DirectoryService);
  convertFileService = inject(ConvertFileService);

  fileConvertQueued = output<IFile>();
  closeOutput = output({
    alias: 'close',
  });

  @ViewChild('modal')
  protected _modal!: ModalComponent;

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

  setOriginalFile(value: IFile | null) {
    this._data = new Data();
    this._data.file = value;

    if (value != null) {
      this._data.path = value.path;
      this._data.name = value.file.slice(0, value.file.lastIndexOf('.'));

      this.convertFileService.getConvertFileByOriginalFile(
        this.directoryService.currentDirectory().dir!.dirEnum,
        value!,
        (result) => (this._data.convertFiles = result)
      );
    }
  }

  protected onSave(): void {
    if (this.isValid()) {
      this.convertFileService.addConvertFile(
        this.directoryService.currentDirectory().dir!.dirEnum,
        {
          directoryEnum: this.directoryService.currentDirectory().dir!.dirEnum,
          originalFile: this._data.file,
          convertedFile: {
            path: this._data.path,
            file: this._data.name + '.mp4',
          } as IFile,
          audioStreamNumber: this._data.audioStreamNumber,
        } as ICreateFileConvert,
        (result) => {
          this._data.file!.isConvertQueued = true;
          this.fileConvertQueued.emit(this._data.file!);
        },
        (errors) => (this._data.errors = errors)
      );
    }
  }

  protected isValid(): boolean {
    var errors = new ModelStateErrors();

    if (this._data.path.length == 0) {
      errors.addPropertyError('path', 'Path is required');
    }
    if (this._data.name.length == 0) {
      errors.addPropertyError('name', 'Name is required');
    }
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
  file: IFile | null = null;
  path = '';
  name = '';
  audioStreamNumber = 1;
  convertFiles: IConvertFile[] = [];
  errors: ModelStateErrors | null = null;
}
