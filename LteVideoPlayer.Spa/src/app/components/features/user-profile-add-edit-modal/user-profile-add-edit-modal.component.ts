import { FormsModule } from '@angular/forms';
import { IUserProfile } from './../../../models/models.d';
import { Component, inject, Input, output, ViewChild } from '@angular/core';
import { ModalComponent } from '../../common/modal/modal.component';
import {
  ModelStateError,
  ModelStateErrors,
} from '../../../models/ModelStateErrors';
import { UserProfileService } from '../../../services/api-services/user-profile.service';

@Component({
  selector: 'app-user-profile-add-edit-modal',
  imports: [FormsModule, ModalComponent],
  templateUrl: './user-profile-add-edit-modal.component.html',
  styleUrl: './user-profile-add-edit-modal.component.scss',
})
export class UserProfileAddEditModalComponent {
  userProfileService = inject(UserProfileService);

  @Input() set userProfile(value: IUserProfile | null) {
    this._userProfile = value ?? this.defaultUserProfile();
    this._errors = null;
  }
  userProfileChange = output<IUserProfile>();
  closeOutput = output({
    alias: 'close',
  });

  protected _userProfile: IUserProfile = this.defaultUserProfile();
  protected _errors: ModelStateErrors | null = null;

  @ViewChild('modal')
  modal!: ModalComponent;

  open(): void {
    this.modal.open();
  }

  close(): void {
    this._userProfile = this.defaultUserProfile();
    this._errors = null;
    this.modal.close();
  }

  protected onClose(): void {
    this.closeOutput.emit();
  }

  onSave(): void {
    if (this.isValid()) {
      if (this._userProfile.id == 0) {
        this.userProfileService.createUserProfile(
          this._userProfile,
          (result) => {
            this._userProfile = result;
            this._errors = null;
            this.userProfileChange.emit(result);
          },
          (error) => (this._errors = error)
        );
      } else {
        this.userProfileService.updateUserProfile(
          this._userProfile,
          (result) => {
            this._userProfile = result;
            this._errors = null;
            this.userProfileChange.emit(result);
          },
          (error) => (this._errors = error)
        );
      }
    }
  }

  isValid(): boolean {
    var errors = new ModelStateErrors();

    if (this._userProfile.name.length == 0) {
      errors.addPropertyError('name', 'Name is required');
    }

    if (errors.errors.length > 0) {
      this._errors = errors;
    } else {
      this._errors = null;
    }

    return this._errors == null;
  }

  defaultUserProfile(): IUserProfile {
    return {
      id: 0,
      name: '',
    };
  }
}
