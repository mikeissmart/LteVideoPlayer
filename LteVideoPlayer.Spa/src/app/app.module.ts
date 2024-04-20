import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './components/app/app.component';
import { FormsModule } from '@angular/forms';

import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { ToasterComponent } from './components/toaster/toaster.component';
import { ModalComponent } from './components/modal/modal.component';
import { UserProfileAddEditComponent } from './components/user-profile-add-edit/user-profile-add-edit.component';
import { UserProfileSelectComponent } from './components/user-profile-select/user-profile-select.component';
import { InputValidationComponent } from './components/input-validation/input-validation.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { FileHistoryLastComponent } from './components/file-history-last/file-history-last.component';
import { FileSelectComponent } from './components/file-select/file-select.component';
import { ConvertFileAddComponent } from './components/convert-file-add/convert-file-add.component';
import { ConvertFileListComponent } from './components/convert-file-list/convert-file-list.component';
import { ConvertFileListAllComponent } from './components/convert-file-list-all/convert-file-list-all.component';
import { ConvertFileAddManyComponent } from './components/convert-file-add-many/convert-file-add-many.component';
import { VideoPlayerComponent } from './components/video-player/video-player.component';
import { RemoteComponent } from './components/remote/remote.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ToasterComponent,
    ModalComponent,
    UserProfileAddEditComponent,
    UserProfileSelectComponent,
    InputValidationComponent,
    DashboardComponent,
    FileHistoryLastComponent,
    FileSelectComponent,
    ConvertFileAddComponent,
    ConvertFileListComponent,
    ConvertFileListAllComponent,
    ConvertFileAddManyComponent,
    VideoPlayerComponent,
    RemoteComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    NgbModule,
    NgxSpinnerModule,
  ],
  providers: [NgbActiveModal],
  bootstrap: [AppComponent],
})
export class AppModule {}
