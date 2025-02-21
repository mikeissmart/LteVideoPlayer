import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { NavMenuComponent } from './components/core/nav-menu/nav-menu.component';
import { ToasterComponent } from './components/core/toaster/toaster.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    NgxSpinnerComponent,
    NavMenuComponent,
    ToasterComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'LteVideoPlayer.Spa';
}
