import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {ThreadCreationData} from "../../../interfaces/thread-creation-data";

@Component({
  selector: 'app-new-thread-dialog',
  templateUrl: './new-thread-dialog.component.html',
  styleUrls: ['./new-thread-dialog.component.scss']
})
export class NewThreadDialogComponent implements OnInit {

  threadCreationData: ThreadCreationData = {
    topic: ''
  }

  constructor(private dialogRef: MatDialogRef<NewThreadDialogComponent>) {
  }

  ngOnInit(): void {
  }

  create() {
    this.threadCreationData.topic = this.threadCreationData.topic.trim();
    this.dialogRef.close(this.threadCreationData);
  }
}
