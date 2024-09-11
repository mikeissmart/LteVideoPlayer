import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { IDirDto, IFileDto } from 'src/app/models/models';
import { FileSelectComponent } from '../file-select/file-select.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  locationChangeSub: any;
  isAdmin = false;
  isStaging = false;
  showVideoSelect = true;
  shareConnectStatus = '';
  hideConnectStatus = true;
  queryDir = '';
  queryFile: string | null = null;

  @ViewChild('fileSelect')
  fileSelect: FileSelectComponent | null = null;

  constructor(
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    public userProfileService: UserProfileService
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.isAdmin = params.has('admin');
      this.queryDir = params.has('dir') ? params.get('dir')! : '';
      this.queryFile = params.get('file');
    });
  }

  /**
   *
   * @param filePathName Leave as null if only updating FileSelectComponent.CurrentDirPathName
   */
  onDirOrFileChange(filePathName: string | null): void {
    var params = {
      admin: this.isAdmin ? this.isAdmin : null,
      staging: this.fileSelect!.isStaging ? this.fileSelect!.isStaging : null,
      dir:
        this.fileSelect!.currentDirPathName != ''
          ? this.fileSelect!.currentDirPathName
          : null,
      file: filePathName,
    };

    this.router.navigate([''], { queryParams: params });
  }
}
