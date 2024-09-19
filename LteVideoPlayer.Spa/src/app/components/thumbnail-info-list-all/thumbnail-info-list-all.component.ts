import { Component, OnInit, ViewChild } from '@angular/core';
import { IFileDto, IThumbnailErrorDto } from 'src/app/models/models';
import { ThumbnailService } from 'src/app/services/api-services/thumbnail.service';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-thumbnail-info-list-all',
  templateUrl: './thumbnail-info-list-all.component.html',
  styleUrls: ['./thumbnail-info-list-all.component.scss'],
})
export class ThumbnailInfoListAllComponent implements OnInit {
  workingThumbnail = '';
  thumbnailErrors: IThumbnailErrorDto[] = [];

  @ViewChild('workingThumbnailModal')
  workingThumbnailModal: ModalComponent | null = null;

  constructor(private readonly thumbnailService: ThumbnailService) {}

  ngOnInit(): void {}

  public refreshThumbnailInfos(): void {
    this.thumbnailService.getWorkingThumbnail((result) => {
      this.workingThumbnail = result.data!;
      this.thumbnailService.getThumbnailErrors((result) => {
        this.thumbnailErrors = result;
        this.workingThumbnailModal?.openModal();
      });
    });
  }

  getWorkingThumbnail(): void {
    this.thumbnailService.getWorkingThumbnail((result) => {
      this.workingThumbnail = result.data!;
    });
  }

  getThumbnailErrors(): void {
    this.thumbnailService.getThumbnailErrors((result) => {
      this.thumbnailErrors = result;
    });
  }

  deleteThumbnailError(thumbnailError: IThumbnailErrorDto): void {
    this.thumbnailService.deleteThumbnailError(thumbnailError.file!, () => {
      this.getWorkingThumbnail();
      this.getThumbnailErrors();
    });
  }

  deleteManyThumbnailErrors(): void {
    var thumbnails: IFileDto[] = [];

    this.thumbnailErrors.forEach((x) => thumbnails.push(x.file!));

    this.thumbnailService.deleteManyThumbnailErrors(thumbnails, () => {
      this.getWorkingThumbnail();
      this.getThumbnailErrors();
    });
  }
}
