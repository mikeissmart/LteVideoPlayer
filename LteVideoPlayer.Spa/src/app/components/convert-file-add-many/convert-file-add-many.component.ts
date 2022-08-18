import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ICreateConvertDto, IDirDto, IFileDto } from 'src/app/models/models';
import { ConvertFileService } from 'src/app/services/api-services/convert-file.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';

@Component({
  selector: 'app-convert-file-add-many',
  templateUrl: './convert-file-add-many.component.html',
  styleUrls: ['./convert-file-add-many.component.scss'],
})
export class ConvertFileAddManyComponent implements OnInit {
  errors: ModelStateErrors | null = null;
  originalPath = '';
  convertPath = '';
  convertItems: any[] = [];

  @Output()
  onCovertFilesAllSaved = new EventEmitter();
  @Output()
  onCovertFileQueued = new EventEmitter<string>();
  @Output()
  onCancel = new EventEmitter();

  constructor(private readonly convertFileService: ConvertFileService) {}

  ngOnInit(): void {}

  setOriginals(dirPathName: string, files: IFileDto[]): void {
    this.convertItems = [];
    this.originalPath = dirPathName;
    this.convertPath = dirPathName;

    files
      .filter((x) => !x.convertQueued!)
      .forEach((x, i) => {
        this.convertItems.push({
          index: i,
          skip: false,
          originalName: x.fileName!,
          convertName: `Episode ${i < 9 ? '0' : ''}${i + 1}`,
          appendConvertName: '',
        });
      });
  }

  onSkipChange(index: number): void {
    this.convertItems[index].skip = !this.convertItems[index].skip;

    let e = 1;
    for (let i = 0; i < this.convertItems.length; i++) {
      this.convertItems[i].convertName =
        this.convertItems[i].skip || this.convertItems[i].convertQueued
          ? `Episode ${e < 10 ? '0' : ''}${e++}`
          : 'N/A';
    }
  }

  saveConvertFiles(): void {
    this.errors = null;

    this.convertItems.forEach((x) => {
      if (!x.skip) {
        this.convertFileService.addConvert(
          {
            originalFile: {
              fileName: x.originalName,
              filePath: this.originalPath,
            } as IFileDto,
            convertedFile: {
              fileName: `${x.convertName}${x.appendConvertName}.mp4`,
              filePath:
                this.convertPath +
                (this.convertPath[this.convertPath.length - 1] != '\\'
                  ? '\\'
                  : ''),
            } as IFileDto,
          } as ICreateConvertDto,
          (result) => {
            this.convertItems = this.convertItems.filter(
              (y) => y.index != x.index
            );
            this.onCovertFileQueued.emit(x.originalName);
            this.processCompletedConverts();
          },
          (error) => {
            if (this.errors == null) {
              this.errors = error;
            } else {
              error?.errors.forEach((x) => {
                const curErrors = this.errors!.errors.filter(
                  (y) => y.property == x.property
                );
                if (curErrors.length == 0) {
                  // new prop error
                  this.errors!.errors.push(x);
                } else {
                  // append error desc
                  curErrors[0].descriptions.push(...x.descriptions);
                }
              });
            }
          }
        );
      }
    });
  }

  processCompletedConverts(): void {
    if (this.convertItems.length == 0) {
      // all succedded
      this.onCovertFilesAllSaved.emit();
    }
  }
}
