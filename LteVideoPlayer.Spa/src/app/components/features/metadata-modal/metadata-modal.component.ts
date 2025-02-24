import { IFile } from '../../../models/models';
import { DirectoryService } from '../../../services/api-services/directory.service';
import { ModalComponent } from '../../common/modal/modal.component';
import { ConvertFileService } from './../../../services/api-services/convert-file.service';
import { Component, inject, Input, output, ViewChild } from '@angular/core';

@Component({
  selector: 'app-metadata-modal',
  imports: [ModalComponent],
  templateUrl: './metadata-modal.component.html',
  styleUrl: './metadata-modal.component.scss',
})
export class MetadataModalComponent {
  convertFileService = inject(ConvertFileService);
  directoryService = inject(DirectoryService);

  closeOutput = output({
    alias: 'close',
  });

  protected _file: IFile | null = null;
  protected _metadataHtml = '';

  @Input() set file(value: IFile | null) {
    this._file = null;
    this._metadataHtml = '';
    if (value != null) {
      this.convertFileService.getVideoMetadata(
        this.directoryService.currentDirectory().dir!.dirEnum,
        value,
        (result) => {
          if (result.output.length > 0) {
            this._metadataHtml = result.output;
          } else {
            this._metadataHtml = result.error;
          }
          this._metadataHtml = this._metadataHtml.replace(
            /stream/gi,
            '<span class="bg-warning text-black">STREAM</span>'
          );
        }
      );
    }
  }

  @ViewChild('modal')
  protected _modal!: ModalComponent;

  open(): void {
    this._modal.open();
  }

  close(): void {
    this._modal.close();
  }

  protected onClose(): void {
    this.closeOutput.emit();
  }
}
