import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DialogData} from "../dialog/dialog.component";

@Component({
  selector: 'app-text-dialog',
  templateUrl: './text-dialog.component.html',
  styleUrls: ['./text-dialog.component.scss']
})
export class TextDialogComponent {
  public constructor(
    public dialogRef: MatDialogRef<TextDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
  ) {}
}
