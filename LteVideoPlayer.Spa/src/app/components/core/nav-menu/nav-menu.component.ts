import { IDirectoryInfo, IUserProfile } from './../../../models/models.d';
import { CommonModule } from '@angular/common';
import { Component, effect, inject, OnInit } from '@angular/core';
import {
  NgbDropdownModule,
  NgbModule,
  NgbNavModule,
} from '@ng-bootstrap/ng-bootstrap';
import { DirectoryService } from '../../../services/api-services/directory.service';
import { UserProfileService } from '../../../services/api-services/user-profile.service';

@Component({
  selector: 'app-nav-menu',
  imports: [NgbModule, CommonModule, NgbNavModule, NgbDropdownModule],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss',
})
export class NavMenuComponent implements OnInit {
  directoryService = inject(DirectoryService);
  userProfileService = inject(UserProfileService);

  isCollapsed = true;
  userProfile: IUserProfile | null = null;
  directories: IDirectoryInfo[] = [];

  constructor() {
    effect(() => {
      this.userProfile = this.userProfileService.currentUserProfile();
    });
  }

  ngOnInit(): void {
    this.directoryService.getDirectories((result) => {
      this.directories = result;
    });
  }

  logout(): void {
    this.userProfileService.setCurrentUserProfile(null);
    this.directoryService.updateCurrentDirectory({
      dir: null,
      path: '',
      file: '',
      isAdmin: this.directoryService.currentDirectory().isAdmin,
    });
  }

  onChangeDirectory(dir: IDirectoryInfo): void {
    this.directoryService.updateCurrentDirectory({
      dir: dir,
      path: '',
      file: '',
      isAdmin: this.directoryService.currentDirectory().isAdmin,
    });
  }

  selectedClass(item: IDirectoryInfo): string {
    if (
      this.directoryService.currentDirectory()?.dir?.friendlyName ==
      item.friendlyName
    ) {
      return ' btn-outline-primary';
    }

    return ' btn-outline-light';
  }
}
