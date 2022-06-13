import { Component } from '@angular/core';
import { UserProfileService } from 'src/app/services/api-services/user-profile.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(public userProfileService: UserProfileService) {}
}
