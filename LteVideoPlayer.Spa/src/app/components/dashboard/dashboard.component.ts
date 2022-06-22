import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
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
    public userProfileService: UserProfileService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.router.url.includes('admin');
  }
}
