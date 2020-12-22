import { ErrorHandler, Injectable } from '@angular/core';
import { ErrorDialogService } from '../../modules/shared/errors/error-dialog.service';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private errorDialogService: ErrorDialogService) {
  }

  handleError(error: Error) {
    this.errorDialogService.openErrorDialog(
      error.stack || 'Undefined client error',
      'TypeScript'
    );
    console.error(error);
  }
}
