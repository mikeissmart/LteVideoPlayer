import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { IConvertFileDto } from 'src/app/models/models';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-convert-file-list',
  templateUrl: './convert-file-list.component.html',
  styleUrls: ['./convert-file-list.component.scss'],
})
export class ConvertFileListComponent implements OnInit {
  sortedConvertFiles: IConvertFileDto[] = [];
  selectedCovertFile: IConvertFileDto | null = null;
  sortProp = 'CreatedDate';
  sortAsc = false;

  @ViewChild('viewOutput')
  viewOutput: ModalComponent | null = null;

  @Input()
  public set convertFiles(v: IConvertFileDto[]) {
    this.sortedConvertFiles = v;
    this.applySort();
  }

  constructor() {}

  ngOnInit(): void {}

  applySort(): void {
    if (this.sortProp == 'CreatedDate') {
      this.sortedConvertFiles = this.sortedConvertFiles.sort((x, y) => {
        return this.toDate(x.createdDate!) > this.toDate(y.createdDate!)
          ? 1
          : -1;
      });
    } /*else if (this.sortProp == 'ProcessTime') {
      this.sortedConvertFiles = this.sortedConvertFiles.sort((x, y) => {
        const xTime =
          x.endedDate != null
            ? this.toTime(
                this.toDate(x.endedDate!).getTime() -
                  this.toDate(x.startedDate!).getTime()
              ).getTime()
            : 0;
        const yTime =
          y.endedDate != null
            ? this.toTime(
                this.toDate(y.endedDate!).getTime() -
                  this.toDate(y.startedDate!).getTime()
              ).getTime()
            : 0;

        return xTime > yTime ? 1 : -1;
      });
    }*/

    if (!this.sortAsc) {
      this.sortedConvertFiles = this.sortedConvertFiles.reverse();
    }
  }

  selectConvertFile(convertFile: IConvertFileDto): void {
    this.selectedCovertFile = convertFile;
    this.viewOutput?.openModal();
  }

  onCloseModal(): void {
    this.selectedCovertFile = null;
  }

  toDate(value: any): Date {
    return new Date(value);
  }

  toTime(value: any): Date {
    const date = new Date(0, 0, 0);
    date.setMilliseconds(value);

    return date;
  }

  sortCreatedDate(): void {
    if (this.sortProp == 'CreatedDate') {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortProp = 'CreatedDate';
      this.sortAsc = true;
    }

    this.applySort();
  }

  sortProcessTime(): void {
    if (this.sortProp == 'ProcessTime') {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortProp = 'ProcessTime';
      this.sortAsc = false;
    }

    this.applySort();
  }
}
