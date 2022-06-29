import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {PostWithDetails} from "../../../interfaces/post-with-details";
import {PostUpdateData} from "../../../interfaces/post-update-data";

@Component({
  selector: 'app-edit-post-dialog',
  templateUrl: './edit-post-dialog.component.html',
  styleUrls: ['./edit-post-dialog.component.scss']
})
export class EditPostDialogComponent implements OnInit {

  postUpdateData: PostUpdateData = {
    content: this.post.content
  }

  constructor(private dialogRef: MatDialogRef<EditPostDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private post: PostWithDetails) {
  }

  ngOnInit(): void {
  }

  save() {
    this.postUpdateData.content = this.postUpdateData.content.trim();

    if (this.postUpdateData.content === this.post.content) {
      this.dialogRef.close();
      return;
    }

    this.dialogRef.close(this.postUpdateData);
  }
}
