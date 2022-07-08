import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {ThreadCreationData} from "../../../interfaces/thread-creation-data";
import {ThreadWithDetails} from "../../../interfaces/thread-with-details";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../../services/notification.service";

@Component({
  selector: 'app-new-thread-dialog',
  templateUrl: './new-thread-dialog.component.html',
  styleUrls: ['./new-thread-dialog.component.scss']
})
export class NewThreadDialogComponent implements OnInit {

  threadCreationData: ThreadCreationData = {
    topic: ''
  }

  inProgress = false;

  constructor(private dialogRef: MatDialogRef<NewThreadDialogComponent>,
              private api: HttpClient,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    this.inProgress = true;
    this.dialogRef.disableClose = true;

    this.threadCreationData.topic = this.threadCreationData.topic.trim();

    this.createNewThread(this.threadCreationData);
  }

  createNewThread(data: ThreadCreationData) {
    this.api.post<ThreadWithDetails>('threads', data)
      .subscribe({
        next: thread => {
          this.dialogRef.close(thread)
          this.ns.notifySuccess("New thread has been created");
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
