import { Injectable } from '@angular/core';
import { IUserProfileDto } from 'src/app/models/models';
import { ApiHttpService } from '../http/api-http.service';
import { ModelStateErrors } from '../http/ModelStateErrors';

@Injectable({
  providedIn: 'root',
})
export class UserProfileService {
  private baseUri = 'Userprofile/';
  private localUserProfile = 'userProfile';

  constructor(private readonly httpClient: ApiHttpService) {}

  getCurrentUserProfile(): IUserProfileDto | null {
    const local = localStorage.getItem(this.localUserProfile);
    if (local == undefined || local == null) {
      return null;
    }

    return JSON.parse(local) as IUserProfileDto;
  }

  setCurrentUserProfile(userProfile: IUserProfileDto | null): void {
    if (userProfile == null) {
      localStorage.removeItem(this.localUserProfile);
    } else {
      localStorage.setItem(this.localUserProfile, JSON.stringify(userProfile));
    }
  }

  getAllUserProfiles(
    callback: (userProfiles: IUserProfileDto[]) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.get<IUserProfileDto[]>(
      this.baseUri + 'GetAllUserProfiles',
      callback,
      errorCallback
    );
  }

  getUserProfileById(
    userProfileId: number,
    callback: (userProfile: IUserProfileDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.get<IUserProfileDto>(
      this.baseUri + 'GetUserProfileById?userProfileId=' + userProfileId,
      callback,
      errorCallback
    );
  }

  createUserProfile(
    userProfile: IUserProfileDto,
    callback: (userProfile: IUserProfileDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IUserProfileDto>(
      this.baseUri + 'CreateUserProfile',
      userProfile,
      callback,
      errorCallback
    );
  }

  updateUserProfile(
    userProfile: IUserProfileDto,
    callback: (userProfile: IUserProfileDto) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<IUserProfileDto>(
      this.baseUri + 'UpdateUserProfile',
      userProfile,
      callback,
      errorCallback
    );
  }

  deleteUserProfile(
    userProfile: IUserProfileDto,
    callback: (isSuccessful: boolean) => void,
    errorCallback?: (errors: ModelStateErrors | null) => void
  ): void {
    this.httpClient.post<boolean>(
      this.baseUri + 'DeleteUserProfile',
      userProfile.id,
      callback,
      errorCallback
    );
  }
}
