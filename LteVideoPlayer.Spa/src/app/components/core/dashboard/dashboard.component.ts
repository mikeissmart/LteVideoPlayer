import { Component, inject, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/api-services/user-profile.service';
import { UserProfileSelectComponent } from '../../features/user-profile-select/user-profile-select.component';
import { DirectoryListComponent } from '../../features/directory-list/directory-list.component';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { IDirectoryInfo } from '../../../models/models';
import { DirPathFile } from '../../../models/view-models';
import { DirectoryService } from '../../../services/api-services/directory.service';
import { ToasterService } from '../../../services/toaster/toaster.service';

@Component({
  selector: 'app-dashboard',
  imports: [UserProfileSelectComponent, DirectoryListComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  activeRoute = inject(ActivatedRoute);
  directoryService = inject(DirectoryService);
  userProfileService = inject(UserProfileService);
  titleService = inject(Title);
  toasterService = inject(ToasterService);

  prevDirPathFile = new DirPathFile();
  directories: IDirectoryInfo[] = [];

  ngOnInit(): void {
    const userProfile = this.userProfileService.currentUserProfile();
    if (userProfile != null) {
      this.userProfileService.getUserProfileById(userProfile.id!, (result) =>
        this.userProfileService.setCurrentUserProfile(result)
      );
    }
    this.directoryService.getDirectories((result) => {
      this.directories = result;
    });
    this.activeRoute.queryParamMap.subscribe((params) => {
      const isAdmin = true; /*params.has('admin')
        ? params.get('admin')! == 'true'
        : false;*/
      const directory = params.has('directory')
        ? params.get('directory')!
        : null;
      const path = params.has('path') ? params.get('path')! : '';
      const file = params.has('file') ? params.get('file')! : '';
      this.setRouteParams(directory, path, file, isAdmin);
    });
  }

  updateTitle(title: string | null): void {
    if (title != null) {
      this.titleService.setTitle(`LteVideoPlayer - ${title}`);
    } else {
      this.titleService.setTitle('LteVideoPlayer');
    }
  }

  setRouteParams(
    friendlyName: string | null,
    path: string,
    file: string,
    isAdmin: boolean
  ): void {
    if (this.directories.length == 0) {
      this.directoryService.getDirectories((result) => {
        this.directories = result;
        this.setRouteParams(friendlyName, path, file, isAdmin);
      });
    } else {
      const currentDirectory = this.directoryService.currentDirectory();
      const foundDir = this.directories.find(
        (x) => x.friendlyName == friendlyName
      );

      if (friendlyName == null) {
        this.directoryService.updateCurrentDirectory({
          dir: this.directories[0],
          path: '',
          file: '',
          isAdmin: isAdmin,
        });
      } else if (
        foundDir == null ||
        (friendlyName != currentDirectory?.dir?.friendlyName &&
          foundDir?.adminViewOnly &&
          !isAdmin)
      ) {
        this.toasterService.showError(`Unknown directory ${friendlyName}`);
        this.directoryService.updateCurrentDirectory({
          dir: this.directories.filter((x) => !x.adminViewOnly)[0],
          path: '',
          file: '',
          isAdmin: isAdmin,
        });
      } else {
        const newDirPathFile = {
          dir: foundDir,
          isAdmin: isAdmin,
          path: path,
          file: file,
        } as DirPathFile;

        if (
          newDirPathFile.dir!.friendlyName !=
            this.prevDirPathFile.dir?.friendlyName ||
          newDirPathFile.path != this.prevDirPathFile.path ||
          newDirPathFile.file != this.prevDirPathFile.file
        ) {
          this.directoryService.currentDirectory.set(newDirPathFile);
        }

        var title = friendlyName!;
        if (path.length > 0) {
          title += `/${path}`;
        }
        if (file.length > 0) {
          title += `/${file}`;
        }

        this.updateTitle(title);
      }
    }
  }

  setDirectoryByFriendlyName(
    friendlyName: string | null,
    newDirPathFile: DirPathFile
  ): boolean {
    const currentDirPathFile = this.directoryService.currentDirectory();
    if (friendlyName == null && currentDirPathFile?.dir?.friendlyName == null) {
      this.directoryService.updateCurrentDirectory({
        dir: this.directories[0],
        path: '',
        file: '',
        isAdmin: newDirPathFile.isAdmin,
      });
    } else if (friendlyName != currentDirPathFile?.dir?.friendlyName) {
      const currentDir =
        this.directories.find((x) => x.friendlyName == friendlyName) ?? null;
      if (currentDir == null) {
        var i = 0;
        i = 1;
      }
      if (
        currentDir == null ||
        (currentDir == null && friendlyName != null) ||
        (currentDir!.adminViewOnly && newDirPathFile.isAdmin)
      ) {
        this.toasterService.showError(`Unknown directory ${friendlyName}`);
        this.directoryService.updateCurrentDirectory({
          dir: this.directories[0],
          path: '',
          file: '',
          isAdmin: newDirPathFile.isAdmin,
        });
        return false;
      } else {
        newDirPathFile.dir = currentDir;
      }
    }

    return true;
  }
}
