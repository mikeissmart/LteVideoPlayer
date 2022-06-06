import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { VideoSelectComponent } from './components/video-select/video-select.component';

const routes: Routes = [{ path: '', component: VideoSelectComponent }];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      useHash: false,
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
