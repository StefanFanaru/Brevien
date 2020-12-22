import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DialogInjectableComponent } from '../components/dialog-injectable/dialog-injectable.component';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  constructor(public dialog: MatDialog) {
  }

  openDialog(title: string, message: string) {
    this.dialog.open(DialogInjectableComponent, {
      data: {
        title: title,
        message: message
      }
    });
  }
}
