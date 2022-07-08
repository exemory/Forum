import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {PostWithDetails} from "../../../interfaces/post-with-details";
import {PostUpdateData} from "../../../interfaces/post-update-data";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../../services/notification.service";

@Component({
  selector: 'app-edit-post-dialog',
  templateUrl: './edit-post-dialog.component.html',
  styleUrls: ['./edit-post-dialog.component.scss']
})
export class EditPostDialogComponent implements OnInit {

  postUpdateData: PostUpdateData = {
    content: this.post.content
  }

  inProgress = false;

  constructor(private dialogRef: MatDialogRef<EditPostDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private post: PostWithDetails,
              private api: HttpClient,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    this.inProgress = true;
    this.dialogRef.disableClose = true;

    this.postUpdateData.content = this.postUpdateData.content.trim();

    this.updatePost(this.postUpdateData);
  }

  updatePost(data: PostUpdateData) {
    this.api.put(`posts/${this.post.id}`, data)
      .subscribe(
        {
          next: () => {
            this.dialogRef.close(data);
            this.ns.notifySuccess(`Post has been updated`);
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
