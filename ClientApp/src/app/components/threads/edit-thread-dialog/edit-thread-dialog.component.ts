import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {ThreadWithDetails} from "../../../interfaces/thread-with-details";
import {ThreadUpdateData} from "../../../interfaces/thread-update-data";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../../services/notification.service";

@Component({
  selector: 'app-edit-thread-dialog',
  templateUrl: './edit-thread-dialog.component.html',
  styleUrls: ['./edit-thread-dialog.component.scss']
})
export class EditThreadDialogComponent implements OnInit {

  threadUpdateData: ThreadUpdateData = {
    topic: this.thread.topic
  }

  inProgress = false;

  constructor(private dialogRef: MatDialogRef<EditThreadDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private thread: ThreadWithDetails,
              private api: HttpClient,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
  }


  onSubmit() {
    this.inProgress = true;
    this.dialogRef.disableClose = true;

    this.threadUpdateData.topic = this.threadUpdateData.topic.trim();

    this.updateThread(this.threadUpdateData);
  }

  updateThread(data: ThreadUpdateData) {
    this.api.put(`threads/${this.thread.id}`, data)
      .subscribe(
        {
          next: () => {
            this.dialogRef.close(data);
            this.ns.notifySuccess(`Thread has been updated`);
          },
          error: err => {
            this.inProgress = false;
            this.dialogRef.disableClose = false;
            this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
          }
        }
      );
  }
}
