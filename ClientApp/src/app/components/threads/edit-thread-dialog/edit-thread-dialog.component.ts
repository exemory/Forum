import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {ThreadWithDetails} from "../../../interfaces/thread-with-details";
import {ThreadUpdateData} from "../../../interfaces/thread-update-data";

@Component({
  selector: 'app-edit-thread-dialog',
  templateUrl: './edit-thread-dialog.component.html',
  styleUrls: ['./edit-thread-dialog.component.scss']
})
export class EditThreadDialogComponent implements OnInit {

  threadUpdateData: ThreadUpdateData = {
    topic: this.thread.topic
  }

  constructor(private dialogRef: MatDialogRef<EditThreadDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private thread: ThreadWithDetails) {
  }

  ngOnInit(): void {
  }


  save() {
    this.threadUpdateData.topic = this.threadUpdateData.topic.trim();

    if (this.threadUpdateData.topic === this.thread.topic) {
      this.dialogRef.close();
      return;
    }

    this.dialogRef.close(this.threadUpdateData);
  }
}
