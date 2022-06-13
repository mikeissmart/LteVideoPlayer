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
  filterPathName = '';
  hasError = -1;

  public get filteredConvertFiles(): IConvertFileDto[] {
    return this.convertFiles.filter((x) => {
      const checkFilter = x
        .originalFile!.filePathName!.toLocaleLowerCase()
        .includes(this.filterPathName.toLocaleLowerCase());
      const checkError =
        this.hasError == -1 ||
        (this.hasError == 1 && x.errored!) ||
        (this.hasError == 0 && !x.errored!);

      return checkFilter && checkError;
    });
  }

  constructor(private readonly convertFileService: ConvertFileService) {}

  ngOnInit(): void {}

  refreshConvertFiles(): void {
    this.convertFileService.getAllConvertFiles(
      (result) => (this.convertFiles = result)
    );
  }
}
