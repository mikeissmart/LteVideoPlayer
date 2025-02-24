import { ThumbnailService } from '../../../services/api-services/thumbnail.service';
import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, output, ViewChild } from '@angular/core';
import { IThumbnailError } from '../../../models/models';
import { ModalComponent } from '../../common/modal/modal.component';

@Component({
  selector: 'app-thumbnail-error-list-modal',
  imports: [DatePipe, ModalComponent],
  templateUrl: './thumbnail-error-list-modal.component.html',
  styleUrl: './thumbnail-error-list-modal.component.scss',
})
export class ThumbnailErrorListModalComponent {
  thumbnailService = inject(ThumbnailService);

  closeOutput = output({
    alias: 'close',
  });

  protected _thumbnailErrors: IThumbnailError[] = [];
  protected _currentThumbnail = '';
  protected _showError: IThumbnailError | null = null;

  @ViewChild('modal')
  modal!: ModalComponent;
  @ViewChild('showErrorModal')
  showErrorModal!: ModalComponent;

  open(): void {
    this.onFetchCurrentThumbnail();
    this.thumbnailService.getAllThumbnailErrors(
      (result) => (this._thumbnailErrors = result)
    );
    this.modal.open();
  }

  close(): void {
    this.modal.close();
  }

  protected onClose(): void {
    this.closeOutput.emit();
  }

  protected onFetchCurrentThumbnail(): void {
    this.thumbnailService.GetCurrentThumbnail(
      (result) => (this._currentThumbnail = result.data)
    );
  }

  protected onShowError(value: IThumbnailError): void {
    this._showError = value;
    this.showErrorModal.open();
  }
}
