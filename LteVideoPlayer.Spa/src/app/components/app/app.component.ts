import { Component, HostListener, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  logText = '';
  showLogs = false;

  constructor() {}

  ngOnInit(): void {}

  @HostListener('window:logger')
  onError() {
    this.logText = (<any>window).logStr;
  }
}
