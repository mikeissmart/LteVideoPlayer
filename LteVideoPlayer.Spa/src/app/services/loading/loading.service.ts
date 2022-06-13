import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  showCounter = 0;

  constructor(private readonly spinner: NgxSpinnerService) {}

  show(): () => void {
    this.showCounter += 1;
    this.spinner.show();
    return () => {
      this.showCounter -= 1;
      if (this.showCounter === 0) {
        this.spinner.hide();
      }
    };
  }
}
