import { Component, OnInit } from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {FormControl} from "@angular/forms";

@Component({
  selector: 'app-new-thread-dialog',
  templateUrl: './new-thread-dialog.component.html',
  styleUrls: ['./new-thread-dialog.component.scss']
})
export class NewThreadDialogComponent implements OnInit {

  topic = new FormControl('');

  constructor(private dialogRef: MatDialogRef<NewThreadDialogComponent>) { }

  ngOnInit(): void {
  }


  create() {
    if (this.topic.invalid) {
      return;
    }

    this.dialogRef.close(this.topic.value);
  }
}
