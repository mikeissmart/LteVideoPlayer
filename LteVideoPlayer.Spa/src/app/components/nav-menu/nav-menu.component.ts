import { Component, OnInit, ViewChild } from '@angular/core';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { ModalComponent } from '../modal/modal.component';
import { RemoteHubService } from 'src/app/services/hubs/remote-hub.service';
import { RemoteComponent } from '../remote/remote.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
})
export class NavMenuComponent implements OnInit {
  isCollapsed = true;
  myChannel = 0;

  @ViewChild('remote')
  remote: RemoteComponent | null = null;

  constructor(
    public userProfileService: UserProfileService,
    private readonly remoteHubService: RemoteHubService
  ) {}

  ngOnInit(): void {
    const currentUserProfile = this.userProfileService.getCurrentUserProfile();
    if (currentUserProfile != null) {
      this.userProfileService.getUserProfileById(
        currentUserProfile.id!,
        (result) => this.userProfileService.setCurrentUserProfile(result)
      );
    }

    this.remoteHubService.receiveYourChannel(
      (channel) => (this.myChannel = channel)
    );
  }

  openRemote(): void {
    this.remote?.openModal();
  }

  changeUserProfile(): void {
    this.userProfileService.setCurrentUserProfile(null);
  }
}
