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
    private readonly activeRoute: ActivatedRoute,
    public userProfileService: UserProfileService
  ) {}

  ngOnInit(): void {
    this.activeRoute.queryParamMap.subscribe((params) => {
      this.isAdmin = params.has('admin');
      this.isStaging = params.has('staging');
      var dir = params.has('dir') ? params.get('dir')! : '';
      var file = params.get('file');

      if (this.fileSelect != null) {
        this.fileSelect.routeChangeFetchDirAndFiles(dir, file);
      } else {
        this.queryDir = dir;
        this.queryFile = file;
      }
    });
  }

  generateQueryParams(): any {
    var params = {
      admin: this.isAdmin ? this.isAdmin : null,
      staging: this.fileSelect!.isStagingDir ? this.fileSelect!.isStagingDir : null,
      dir:
        this.fileSelect!.currentDirPathName != ''
          ? this.fileSelect!.currentDirPathName
          : null,
      file: this.queryFile,
    };

    return params;
  }

  /**
   *
   * @param filePathName Leave as null if only updating FileSelectComponent.CurrentDirPathName
   */
  onDirOrFileChange(filePathName: string | null): void {
    this.queryFile = filePathName;

    this.router.navigate([''], { queryParams: this.generateQueryParams() });
  }

  onIsStagingChange(isStagingDir: boolean): void {
    this.isStaging = isStagingDir;

    this.router.navigate([''], { queryParams: this.generateQueryParams() });
  }
}
