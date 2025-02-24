import { ConvertFileService } from './../../../services/api-services/convert-file.service';
import { DatePipe } from '@angular/common';
import { IConvertFile } from './../../../models/models.d';
import { Component, inject, input, OnInit } from '@angular/core';

@Component({
  selector: 'app-convert-file-list',
  imports: [DatePipe],
  templateUrl: './convert-file-list.component.html',
  styleUrl: './convert-file-list.component.scss',
})
export class ConvertFileListComponent implements OnInit {
  convertFileService = inject(ConvertFileService);

  convertFiles = input.required<IConvertFile[]>();

  protected _currentConvert: IConvertFile | null = null;

  ngOnInit(): void {
    this.onFetchCurrentConvert();
  }

  protected onFetchCurrentConvert(): void {
    this.convertFileService.getCurrentConvertFile(
      (result) => (this._currentConvert = result)
    );
  }

  protected toDate(value: any): Date {
    return new Date(value);
  }

  protected toTime(value: any): Date {
    const date = new Date(0, 0, 0);
    date.setMilliseconds(value);

    return date;
  }
}
