import { Component } from '@angular/core';
import { FileHistoryService } from 'src/app/services/api-services/file-history.service';

@Component({
  selector: 'app-file-history-last',
  templateUrl: './file-history-last.component.html',
  styleUrls: ['./file-history-last.component.scss'],
})
export class FileHistoryLastComponent {
  constructor(public fileHistoryService: FileHistoryService) {}
}
