import {Component, OnInit} from '@angular/core';
import {PasswordChangeData} from "../../../interfaces/password-change-data";
import {FormBuilder} from "@angular/forms";
import {MatDialogRef} from "@angular/material/dialog";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../../services/notification.service";
import {ConfirmPasswordStateMatcher} from "../../../shared/confirm-password-state-matcher";
import {confirmPasswordValidator} from "../../../shared/confirm-password-validator";

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.scss']
})
export class ChangePasswordDialogComponent implements OnInit {

  form = this.fb.group({
    currentPassword: [''],
    newPassword: [''],
    confirmPassword: ['']
  }, {validators: confirmPasswordValidator('newPassword', 'confirmPassword')});

  confirmPasswordStateMatcher = new ConfirmPasswordStateMatcher();

  hideCurrentPassword = true;
  hideNewPassword = true;
  hideConfirmPassword = true;

  inProgress = false;

  constructor(private dialogRef: MatDialogRef<ChangePasswordDialogComponent>,
              private fb: FormBuilder,
              private api: HttpClient,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    if (this.form.invalid || this.inProgress) {
      return;
    }

    this.inProgress = true;
    this.dialogRef.disableClose = true;

    let data: PasswordChangeData = {
      currentPassword: this.form.get('currentPassword')?.value,
      newPassword: this.form.get('newPassword')?.value
    }

    this.changePassword(data);
  }

  changePassword(data: PasswordChangeData) {
    this.api.put('account/password', data)
      .subscribe({
        next: () => {
          this.dialogRef.close();
          this.ns.notifySuccess("Password has been changed");
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
