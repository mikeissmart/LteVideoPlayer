import { Component, OnInit, ViewChild } from '@angular/core';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { FileSelectComponent } from '../file-select/file-select.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {

  @ViewChild('fileSelect')
  fileSelect: FileSelectComponent | null = null;

  constructor(
    public userProfileService: UserProfileService
  ) {}

  ngOnInit(): void {
  }
}
