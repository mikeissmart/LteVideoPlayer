import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingService } from '../loading/loading.service';
import { HttpService } from './http.service';
import { environment } from '../../../environments/environment';
import { ModelStateErrors } from '../../models/ModelStateErrors';

@Injectable({
  providedIn: 'root',
})
export class ApiHttpService extends HttpService<ModelStateErrors> {
  public serverErrorMessage = '';

  router = inject(Router);
  loadingService = inject(LoadingService);
  httpClient = inject(HttpClient);

  constructor() {
    super();

    this.initialize(
      this.loadingService,
      this.httpClient,
      environment.apiUrl,
      this.getHeaderOptions,
      this.handleErrors
    );
  }

  private getHeaderOptions(): { headers: HttpHeaders } {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    };
  }

  private handleErrors(httpError: HttpErrorResponse): ModelStateErrors | null {
    const appError = httpError.headers.get('Application-Error');
    if (appError) {
      throw appError;
    }
    switch (httpError.status) {
      case 500: // server error
        this.serverErrorMessage = httpError.error;
        this.router.navigateByUrl('/servererror', {
          skipLocationChange: true,
        });
        break;
      case 401: // forbidden
        this.router.navigateByUrl('/forbidden', {
          skipLocationChange: true,
        });
        break;
      case 403: // unauthorized
        this.router.navigateByUrl('/unauthorized', {
          skipLocationChange: true,
        });
        break;
      case 400: // bad request
        if (typeof httpError.error == 'string') {
          console.error(httpError.error);
        } else {
          const modelErrors = ModelStateErrors.convertErrors(httpError.error);
          return modelErrors;
        }
    }
    return null;
  }
}
