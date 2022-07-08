import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {AccountUpdateData} from "../../../interfaces/account-update-data";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../../services/notification.service";
import {UserWithDetails} from "../../../interfaces/user-with-details";

@Component({
  selector: 'app-edit-profile-dialog',
  templateUrl: './edit-profile-dialog.component.html',
  styleUrls: ['./edit-profile-dialog.component.scss']
})
export class EditProfileDialogComponent implements OnInit {

  hideCurrentPassword = true;
  inProgress = false;

  form = this.fb.group({
    username: [this.profileInfo.username, Validators.pattern(/^[a-z\d-._@+]*$/i)],
    email: [this.profileInfo.email],
    name: [this.profileInfo.name, Validators.pattern(/^[a-z ]*$/i)],
    currentPassword: [''],
  });

  constructor(private dialogRef: MatDialogRef<EditProfileDialogComponent>,
              @Inject(MAT_DIALOG_DATA) private profileInfo: UserWithDetails,
              private fb: FormBuilder,
              private api: HttpClient,
              private ns: NotificationService) { }

  ngOnInit(): void {
  }

  onSubmit() {
    if (this.form.invalid || this.inProgress) {
      return;
    }

    this.inProgress = true;
    this.dialogRef.disableClose = true;

    let data: AccountUpdateData = {
      username: this.form.get('username')?.value,
      email: this.form.get('email')?.value,
      name: this.form.get('name')?.value.trim(),
      currentPassword: this.form.get('currentPassword')?.value,
    }

    if (data.name === '') {
      data.name = undefined;
    }

    this.updateProfile(data);
  }

  updateProfile(data: AccountUpdateData) {
    this.api.put('account', data)
      .subscribe({
        next: () => {
          this.dialogRef.close(data);
          this.ns.notifySuccess("Profile has been updated");
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
