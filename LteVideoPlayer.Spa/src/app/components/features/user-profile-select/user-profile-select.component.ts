import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { UserProfileService } from '../../../services/api-services/user-profile.service';
import { UserProfileAddEditModalComponent } from '../user-profile-add-edit-modal/user-profile-add-edit-modal.component';
import { IUserProfile } from '../../../models/models';

@Component({
  selector: 'app-user-profile-select',
  imports: [UserProfileAddEditModalComponent],
  templateUrl: './user-profile-select.component.html',
  styleUrl: './user-profile-select.component.scss',
})
export class UserProfileSelectComponent implements OnInit {
  userProfileService = inject(UserProfileService);

  userProfiles: IUserProfile[] = [];
  addEditUserProfile: IUserProfile | null = null;

  @ViewChild('userProfileAddEditModal')
  userProfileAddEditModal!: UserProfileAddEditModalComponent;

  ngOnInit(): void {
    this.fetchUserProfiles();
  }

  fetchUserProfiles(): void {
    this.userProfileService.getAllUserProfiles(
      (results) => (this.userProfiles = results)
    );
  }

  openUserProfileAddEditModal(value: IUserProfile | null): void {
    this.addEditUserProfile = value;
    this.userProfileAddEditModal.open();
  }

  onUserProfileSave(): void {
    this.userProfileAddEditModal.close();
    this.addEditUserProfile = null;
    this.fetchUserProfiles();
  }

  onUserProfileAddEditModalClose(): void {
    this.addEditUserProfile = null;
  }
}
