import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ToasterService {
  toasts = signal<any[]>([]);

  show(message: string, classNames: string) {
    this.toasts.update((x) => [...x, { message, classNames }]);
  }

  showSuccess(message: string): void {
    this.show(message, 'bg-success text-light toast align-items-center host');
  }

  showError(message: string): void {
    this.show(
      message,
      'bg-danger text-light text-light toast align-items-center host'
    );
  }

  showWarning(message: string): void {
    this.show(
      message,
      'bg-warning text-light text-light toast align-items-center host'
    );
  }

  remove(toast: any) {
    this.toasts.update((x) => x.filter((t) => t != toast));
  }
}
