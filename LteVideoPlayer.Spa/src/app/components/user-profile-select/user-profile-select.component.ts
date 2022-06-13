import { Component, OnInit, ViewChild } from '@angular/core';
import { IUserProfileDto } from 'src/app/models/models';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';
import { ModalComponent } from '../modal/modal.component';
import { UserProfileAddEditComponent } from '../user-profile-add-edit/user-profile-add-edit.component';

@Component({
  selector: 'app-user-profile-select',
  templateUrl: './user-profile-select.component.html',
  styleUrls: ['./user-profile-select.component.scss'],
})
export class UserProfileSelectComponent implements OnInit {
  userProfiles: IUserProfileDto[] = [];
  selectedUserProfile: IUserProfileDto = {
    id: 0,
    name: '',
  } as IUserProfileDto;
  isEditing = false;

  @ViewChild('userProfileModal')
  userProfileModal: ModalComponent | null = null;
  @ViewChild('userProfileAddEdd')
  userProfileAddEdd: UserProfileAddEditComponent | null = null;

  constructor(private readonly userProfileService: UserProfileService) {}

  ngOnInit(): void {
    this.fetchAllUserProfiles();
  }

  fetchAllUserProfiles(): void {
    this.userProfileService.getAllUserProfiles(
      (results) => (this.userProfiles = results)
    );
  }

  selectUserProfile(userProfile: IUserProfileDto): void {
    this.userProfileService.setCurrentUserProfile(userProfile);
  }

  newUserProfile(): void {
    this.selectedUserProfile = {
      id: 0,
      name: '',
    } as IUserProfileDto;
    this.userProfileModal?.openModal();
  }

  editUserProfile(userProfile: IUserProfileDto): void {
    this.selectedUserProfile = userProfile;
    this.isEditing = true;
    this.userProfileModal?.openModal();
  }

  saveUserProfile(userProfile: IUserProfileDto): void {
    this.userProfileModal?.closeModal();
    if (!this.isEditing) {
      this.userProfileService.setCurrentUserProfile(userProfile);
    }
  }
}
