import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-dialog-injectable',
  templateUrl: './dialog-injectable.component.html',
  styleUrls: ['./dialog-injectable.component.scss']
})
export class DialogInjectableComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData) {
  }

  ngOnInit(): void {
  }
}

export interface DialogData {
  title: string;
  message: string;
}
