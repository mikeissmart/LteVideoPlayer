import { IUserProfile } from './../../models/models.d';
import { Injectable, signal } from '@angular/core';
import { ModelStateErrors } from '../../models/ModelStateErrors';
import { ApiHttpService } from '../http/api-http.service';

@Injectable({
  providedIn: 'root',
})
export class UserProfileService {
  private baseUri = 'Userprofile/';
  private localUserProfile = 'userProfile';

  currentUserProfile = signal<IUserProfile | null>(null);

  constructor(private readonly httpClient: ApiHttpService) {}

  getUserProfileFromLocal(): IUserProfile | null {
    const local = localStorage.getItem(this.localUserProfile);
    if (local == undefined || local == null) {
      return null;
    }

    return JSON.parse(local!) as IUserProfile;
  }

  setCurrentUserProfile(userProfile: IUserProfile | null): void {
    if (userProfile == null) {
      localStorage.removeItem(this.localUserProfile);
    } else {
      localStorage.setItem(this.localUserProfile, JSON.stringify(userProfile));
    }
    this.currentUserProfile.set(userProfile);
  }

  getAllUserProfiles(callback: (userProfiles: IUserProfile[]) => void): void {
    this.httpClient.get<IUserProfile[]>(
      this.baseUri + 'GetAllUserProfiles',
      callback
    );
  }

  getUserProfileById(
    userProfileId: number,
    callback: (userProfile: IUserProfile) => void
  ): void {
    this.httpClient.get<IUserProfile>(
      this.baseUri + 'GetUserProfileById?userProfileId=' + userProfileId,
      callback
    );
  }

  createUserProfile(
    userProfile: IUserProfile,
    callback: (userProfile: IUserProfile) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IUserProfile>(
      this.baseUri + 'CreateUserProfile',
      userProfile,
      callback,
      errorCallback
    );
  }

  updateUserProfile(
    userProfile: IUserProfile,
    callback: (userProfile: IUserProfile) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IUserProfile>(
      this.baseUri + 'UpdateUserProfile',
      userProfile,
      callback,
      errorCallback
    );
  }

  deleteUserProfile(
    userProfile: IUserProfile,
    callback: (isSuccessful: boolean) => void
  ): void {
    this.httpClient.post<boolean>(
      this.baseUri + 'DeleteUserProfile',
      userProfile.id,
      callback
    );
  }
}
