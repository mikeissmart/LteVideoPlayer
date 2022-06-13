import { Component, OnInit } from '@angular/core';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
})
export class NavMenuComponent implements OnInit {
  isCollapsed = true;

  constructor(public userProfileService: UserProfileService) {}

  ngOnInit(): void {
    const currentUserProfile = this.userProfileService.getCurrentUserProfile();
    if (currentUserProfile != null) {
      this.userProfileService.getUserProfileById(
        currentUserProfile.id!,
        (result) => this.userProfileService.setCurrentUserProfile(result)
      );
    }
  }

  changeUserProfile(): void {
    this.userProfileService.setCurrentUserProfile(null);
  }
}
