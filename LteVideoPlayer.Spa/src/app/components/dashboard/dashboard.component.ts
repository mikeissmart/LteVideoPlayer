import { Component, OnDestroy, OnInit } from '@angular/core';
import { Event, NavigationStart, Router } from '@angular/router';
import { IConvertFileDto } from 'src/app/models/models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  locationChangeSub: any;
  isAdmin = false;
  showVideoSelect = true;

  constructor(private readonly router: Router) {}

  ngOnInit(): void {
    this.isAdmin = this.router.url.includes('admin');
  }

  ngOnDestroy(): void {
    this.locationChangeSub.onUrlChange();
  }
}
