import { Component, OnInit } from '@angular/core';
import {PasswordChangeData} from "../../../interfaces/password-change-data";
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroupDirective, NgForm,
  ValidationErrors,
  ValidatorFn
} from "@angular/forms";
import {ErrorStateMatcher} from "@angular/material/core";
import {MatDialogRef} from "@angular/material/dialog";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../../services/notification.service";

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.scss']
})
export class ChangePasswordDialogComponent implements OnInit {

  passwordsValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    const newPassword = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : {passwordMismatch: true};
  }

  form = this.fb.group({
    currentPassword: [''],
    newPassword: [''],
    confirmPassword: ['']
  }, {validators: this.passwordsValidator});

  confirmPasswordStateMatcher = new ConfirmPasswordStateMatcher();

  hideCurrentPassword = true;
  hideNewPassword = true;
  hideConfirmPassword = true;

  inProgress = false;

  constructor(private dialogRef: MatDialogRef<ChangePasswordDialogComponent>,
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

export class ConfirmPasswordStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    return (!!control?.touched || !!form?.submitted) && !!form?.hasError('passwordMismatch');
  }
}
