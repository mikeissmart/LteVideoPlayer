import { Component, OnInit } from '@angular/core';
import { IConvertFileDto } from 'src/app/models/models';
import { ConvertFileService } from 'src/app/services/api-services/convert-file.service';

@Component({
  selector: 'app-convert-file-list-all',
  templateUrl: './convert-file-list-all.component.html',
  styleUrls: ['./convert-file-list-all.component.scss'],
})
export class ConvertFileListAllComponent implements OnInit {
  convertFiles: IConvertFileDto[] = [];
  currentConvertFiles: IConvertFileDto[] = [];
  hasError = -1;

  constructor(private readonly convertFileService: ConvertFileService) {}

  ngOnInit(): void {}

  refreshConvertFiles(): void {
    this.convertFileService.getAllConvertFiles(
      (result) => (this.convertFiles = result)
    );
    this.convertFileService.getWorkingConvertFiles(
      (result) => (this.currentConvertFiles = result)
    );
  }

  toDate(value: any): Date {
    return new Date(value);
  }

  toTime(value: any): Date {
    const date = new Date(0, 0, 0);
    date.setMilliseconds(value);

    return date;
  }
}
