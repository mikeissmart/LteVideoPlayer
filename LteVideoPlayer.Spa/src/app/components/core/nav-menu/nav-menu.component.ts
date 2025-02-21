import { IDirectoryInfo, IUserProfile } from './../../../models/models.d';
import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  NgbDropdownModule,
  NgbModule,
  NgbNavModule,
} from '@ng-bootstrap/ng-bootstrap';
import { DirectoryService } from '../../../services/api-services/directory.service';
import { UserProfileService } from '../../../services/api-services/user-profile.service';
import { Title } from '@angular/platform-browser';
import { ToasterService } from '../../../services/toaster/toaster.service';

@Component({
  selector: 'app-nav-menu',
  imports: [
    NgbModule,
    CommonModule,
    RouterLink,
    NgbNavModule,
    NgbDropdownModule,
  ],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss',
})
export class NavMenuComponent implements OnInit {
  activeRoute = inject(ActivatedRoute);
  router = inject(Router);
  directoryService = inject(DirectoryService);
  userProfileService = inject(UserProfileService);
  titleService = inject(Title);
  toasterService = inject(ToasterService);

  isCollapsed = true;
  isAdmin = false;
  directories: IDirectoryInfo[] = [];

  ngOnInit(): void {
    const userProfile =
      this.userProfileService.getCurrentUserProfileFromLocal();
    if (userProfile != null) {
      this.userProfileService.getUserProfileById(userProfile.id!, (result) =>
        this.userProfileService.setCurrentUserProfile(result)
      );
    }
    this.directoryService.getDirectories((result) => {
      this.directories = result;
    });
    this.activeRoute.queryParamMap.subscribe((params) => {
      this.isAdmin = params.has('admin')
        ? params.get('admin')! == 'true'
        : false;
      const directory = params.has('directory')
        ? params.get('directory')!
        : null;
      const path = params.has('path') ? params.get('path')! : '';
      const file = params.has('file') ? params.get('file')! : '';
      this.setRouteParams(directory, path, file);
    });
  }

  updateTitle(title: string | null): void {
    if (title != null) {
      this.titleService.setTitle(`LteVideoPlayer - ${title}`);
    } else {
      this.titleService.setTitle('LteVideoPlayer');
    }
  }

  setRouteParams(directory: string | null, path: string, file: string): void {
    if (this.directories.length == 0) {
      this.directoryService.getDirectories((result) => {
        this.directories = result;
        this.setRouteParams(directory, path, file);
      });
    } else {
      if (this.setDirectoryByFriendlyName(directory)) {
        if (this.directoryService.currentDirectory != null) {
          if (path != this.directoryService.currentPath()) {
            this.directoryService.currentPath.set(path);
          }
          if (file != this.directoryService.currentPath()) {
            this.directoryService.currentFile.set(file);
          }
        }

        if (directory != null) {
          var title = directory!;
          if (path.length > 0) {
            title += `/${path}`;
          }
          if (file.length > 0) {
            title += `/${file}`;
          }

          this.updateTitle(title);
        } else {
          this.updateTitle(null);
        }
      }
    }
  }

  setDirectoryByFriendlyName(friendlyName: string | null): boolean {
    if (
      friendlyName !=
      (this.directoryService.currentDirectory()?.friendlyName ?? null)
    ) {
      const currentDir =
        this.directories.find((x) => x.friendlyName == friendlyName) ?? null;
      if (
        (currentDir == null && friendlyName != null) ||
        !currentDir!.adminViewOnly ||
        (currentDir!.adminViewOnly && this.isAdmin)
      ) {
        this.directoryService.currentDirectory.set(null);
        this.toasterService.showError(`Unknown directory ${friendlyName}`);
        this.updateRouteParams(this.isAdmin, null, '', '');
        return false;
      } else {
        this.directoryService.currentDirectory.set(currentDir);
      }
    }

    return true;
  }

  updateRouteParams(
    isAdmin: boolean,
    directoryInfo: IDirectoryInfo | null,
    path: string,
    file: string
  ): void {
    var params = {
      admin: isAdmin ? isAdmin : null,
      directory: directoryInfo?.friendlyName ?? null,
      path: path.length > 0 ? path : null,
      file: file.length > 0 ? file : null,
    };

    this.router.navigate([''], { queryParams: params });
  }

  selectedClass(item: IDirectoryInfo): string {
    if (this.directoryService.currentDirectory() == item) {
      return ' btn-outline-primary';
    }

    return '';
  }
}
