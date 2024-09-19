import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { IThumbnailErrorDto } from 'src/app/models/models';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-thumbnail-info-list',
  templateUrl: './thumbnail-info-list.component.html',
  styleUrls: ['./thumbnail-info-list.component.scss'],
})
export class ThumbnailInfoListComponent implements OnInit {
  sortedThumbnailErrors: IThumbnailErrorDto[] = [];
  selectedThumbnailError: IThumbnailErrorDto | null = null;
  sortProp = 'LastError';
  sortAsc = true;

  @Input()
  public set thumbnailErrors(v: IThumbnailErrorDto[]) {
    this.sortedThumbnailErrors = v;
    this.sortProp = 'LastError';
    this.sortAsc = true;
    this.applySort();
  }

  @ViewChild('disThumbErrorModal')
  disThumbErrorModal: ModalComponent | null = null;

  constructor() {}

  ngOnInit(): void {}

  applySort(): void {
    if (this.sortProp == 'LastError') {
      this.sortedThumbnailErrors = this.sortedThumbnailErrors.sort((x, y) => {
        return this.toDate(x.lastError!) > this.toDate(y.lastError!) ? 1 : -1;
      });
    }

    if (!this.sortAsc) {
      this.sortedThumbnailErrors = this.sortedThumbnailErrors.reverse();
    }
  }

  sortLastError(): void {
    if (this.sortProp == 'LastError') {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortProp = 'LastError';
      this.sortAsc = true;
    }

    this.applySort();
  }

  toDate(value: any): Date {
    return new Date(value);
  }

  displayThumbnailError(thumbnailError: IThumbnailErrorDto): void {
    this.selectedThumbnailError = thumbnailError;
    this.disThumbErrorModal?.openModal();
  }
}
