import { Component, OnDestroy, OnInit } from '@angular/core';
import { Event, NavigationStart, Router } from '@angular/router';
import { IConvertFileDto } from 'src/app/models/models';
import { ShareConnectService } from 'src/app/services/api-services/share-connect.service';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  locationChangeSub: any;
  isAdmin = false;
  showVideoSelect = true;
  shareConnectStatus = '';
  hideConnectStatus = true;

  constructor(
    private readonly router: Router,
    public userProfileService: UserProfileService,
    private readonly shareConnectService: ShareConnectService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.router.url.includes('admin');
    this.shareConnectService.getShareConnectStatus((result) => {
      this.shareConnectStatus = result;
      if (result.length > 0) {
        this.hideConnectStatus = false;
      } else {
        this.shareConnectStatus = 'N/A';
      }
    });
  }
}
