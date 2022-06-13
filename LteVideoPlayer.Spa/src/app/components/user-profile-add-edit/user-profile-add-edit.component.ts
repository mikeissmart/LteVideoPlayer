import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IUserProfileDto } from 'src/app/models/models';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';
import { ToasterService } from 'src/app/services/toaster/toaster.service';

@Component({
  selector: 'app-user-profile-add-edit',
  templateUrl: './user-profile-add-edit.component.html',
  styleUrls: ['./user-profile-add-edit.component.scss'],
})
export class UserProfileAddEditComponent implements OnInit {
  errors: ModelStateErrors | null = null;

  @Input()
  userProfile: IUserProfileDto = {
    id: 0,
    name: '',
  } as IUserProfileDto;
  @Output()
  onUserProfileChange = new EventEmitter<IUserProfileDto>();
  @Output()
  onCancel = new EventEmitter();

  constructor(
    private readonly userProfileService: UserProfileService,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {}

  saveUserProfile(): void {
    if (this.userProfile.id == 0) {
      this.userProfileService.createUserProfile(
        this.userProfile,
        (result) => this.setUserProfile(result),
        (errors) => (this.errors = errors)
      );
    } else {
      this.userProfileService.updateUserProfile(
        this.userProfile,
        (result) => this.setUserProfile(result),
        (errors) => (this.errors = errors)
      );
    }
  }

  setUserProfile(userProfile: IUserProfileDto): void {
    this.toaster.showSuccess('User Profile Saved');
    this.userProfile = userProfile;
    this.onUserProfileChange.emit(userProfile);
  }
}
