import {Observable, Subscription, throwError} from 'rxjs';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import {catchError, finalize} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {ErrorDialogService} from '../../modules/shared/errors/error-dialog.service';
import {SpinnerOverlayService} from '../../modules/shared/loading/loading-dialog.service';
import {ErrorType} from '../../modules/shared/errors/error-dialog/error-dialog.component';

@Injectable()
export class CustomHttpInterceptor implements HttpInterceptor {
  constructor(
    private errorDialogService: ErrorDialogService,
    private readonly spinnerOverlayService: SpinnerOverlayService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // show loading spinner
    const spinnerSubscription: Subscription = this.spinnerOverlayService.spinner$.subscribe();

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let content, validationErrors, exception;
        if (error.error) {
          exception = error.error.Message !== undefined;
          if (error.error?.errors) {
            validationErrors = this.extractValidationErrors(error);
          }
        }

        let errorType: ErrorType = exception
          ? 'Exception'
          : validationErrors
          ? 'ApiValidation'
          : 'Http';

        switch (errorType) {
          case 'Exception':
            content = JSON.stringify(error.error, null, 2);
            break;
          case 'ApiValidation':
            content = validationErrors;
            break;
          case 'Http':
            content =
              error?.error?.message || error?.message || 'Unknown HTTP error';
            break;
        }

        this.errorDialogService.openErrorDialog(
          content,
          errorType,
          error.status
        );
        return throwError(error);
      }),
      finalize(() => {
        // hiding the spinner
        if (spinnerSubscription) {
          spinnerSubscription.unsubscribe();
        }
      })
    ) as Observable<HttpEvent<any>>;
  }

  extractValidationErrors(error: HttpErrorResponse) {
    let errors = error.error.errors;
    if (errors.length > 0) {
      return errors;
    }
  }
}
