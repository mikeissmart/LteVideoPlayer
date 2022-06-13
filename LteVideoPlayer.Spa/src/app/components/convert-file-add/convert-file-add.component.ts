import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  IConvertFileDto,
  ICreateConvertDto,
  IFileDto,
} from 'src/app/models/models';
import { ConvertFileService } from 'src/app/services/api-services/convert-file.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';

@Component({
  selector: 'app-convert-file-add',
  templateUrl: './convert-file-add.component.html',
  styleUrls: ['./convert-file-add.component.scss'],
})
export class ConvertFileAddComponent implements OnInit {
  errors: ModelStateErrors | null = null;
  convertFiles: IConvertFileDto[] = [];
  originalFile: IFileDto | null = null;
  convertFilePath = '';
  convertFileName = '';

  @Output()
  onCovertFileSaved = new EventEmitter<IConvertFileDto>();
  @Output()
  onCancel = new EventEmitter();

  constructor(private readonly convertFileService: ConvertFileService) {}

  ngOnInit(): void {}

  setOriginalFile(file: IFileDto): void {
    this.convertFileService.getConvertFileByOriginalFile(
      file,
      (result) => {
        this.originalFile = file;
        this.convertFilePath = file.filePath!;

        const ext = file.fileName!.lastIndexOf('.');
        this.convertFileName = file.fileName!.slice(0, ext);
        this.convertFiles = result;
      },
      (error) => (this.errors = error)
    );
  }

  saveConvertFile(): void {
    this.convertFileService.addConvert(
      {
        originalFile: this.originalFile,
        convertedFile: {
          fileName: this.convertFileName + '.mp4',
          filePath:
            this.convertFilePath +
            (this.convertFilePath[this.convertFilePath.length - 1] != '\\'
              ? '\\'
              : ''),
        } as IFileDto,
      } as ICreateConvertDto,
      (result) => this.onCovertFileSaved.emit(result),
      (error) => (this.errors = error)
    );
  }
}
