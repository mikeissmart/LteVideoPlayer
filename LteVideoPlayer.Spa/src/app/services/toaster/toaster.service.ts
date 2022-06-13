import { Injectable, TemplateRef } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ToasterService {
  toasts: any[] = [];

  show(textOrTemplate: string | TemplateRef<any>, options: any = {}) {
    this.toasts.push({ textOrTemplate, ...options });
  }

  showSuccess(message: string): void {
    this.show(message, {
      classname: 'bg-success text-light'
    })
  }

  showError(message: string): void {
    this.show(message, {
      classname: 'bg-danger text-light'
    })
  }

  showWarning(message: string): void {
    this.show(message, {
      classname: 'bg-warning text-light'
    })
  }

  remove(toast: any) {
    this.toasts = this.toasts.filter(t => t !== toast);
  }
}
