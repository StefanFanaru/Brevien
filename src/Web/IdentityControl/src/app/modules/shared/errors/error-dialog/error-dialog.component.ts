import { Component, Inject, NgZone } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.scss']
})
export class ErrorDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      content: any;
      type: ErrorType;
      status?: number;
      title: string;
    }, public dialogRef: MatDialogRef<ErrorDialogComponent>, private ngZone: NgZone
  ) {
    // console.log(data)
  }

  close() {
    this.ngZone.run(() => {
      this.dialogRef.close();
    });
  }
}

export type ErrorType = 'Exception' | 'Http' | 'ApiValidation' | 'TypeScript';
