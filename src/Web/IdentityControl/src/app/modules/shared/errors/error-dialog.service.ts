import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ErrorDialogComponent, ErrorType } from './error-dialog/error-dialog.component';

@Injectable()
export class ErrorDialogService {
  private opened = false;

  constructor(public dialog: MatDialog) {
  }

  openErrorDialog(
    content: string,
    type: ErrorType,
    status?: number
  ): void {
    if (!this.opened) {
      this.opened = true;
      let width = this.getWidth(type)
      const dialogRef = this.dialog.open(ErrorDialogComponent, {
        data: { content, type, status },
        width: width,
        disableClose: false,
        hasBackdrop: true
      });

      dialogRef.afterClosed().subscribe(() => {
        this.opened = false;
      });
    }
  }

  getWidth(type: string): string {
    switch (type) {
      case 'Exception':
        return '1200px'
      case 'TypeScript':
        return '900px'
      default:
        return '540px';
    }
  }
}
