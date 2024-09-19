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
  hasError = -1;

  constructor(private readonly convertFileService: ConvertFileService) {}

  ngOnInit(): void {}

  refreshConvertFiles(): void {
    this.convertFileService.getAllConvertFiles(
      (result) => (this.convertFiles = result)
    );
  }
}
