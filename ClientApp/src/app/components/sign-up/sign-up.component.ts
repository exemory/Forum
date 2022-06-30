import {Component, OnInit} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroupDirective, NgForm,
  ValidationErrors,
  ValidatorFn
} from "@angular/forms";
import {NotificationService} from "../../services/notification.service";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {SignUpData} from "../../interfaces/sign-up-data";
import {ErrorStateMatcher} from "@angular/material/core";

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

  passwordsValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : {passwordMismatch: true};
  }

  form = this.fb.group({
    username: [''],
    email: [''],
    name: [''],
    password: [''],
    confirmPassword: ['']
  }, {validators: this.passwordsValidator});

  confirmPasswordStateMatcher = new ConfirmPasswordStateMatcher();

  hidePassword = true;
  hideConfirmPassword = true;
  inProgress = false;

  constructor(private fb: FormBuilder,
              private api: HttpClient,
              private ns: NotificationService,
              private router: Router) {
  }

  ngOnInit(): void {
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.inProgress = true;

    let data: SignUpData = {
      username: this.form.get('username')?.value,
      email: this.form.get('email')?.value,
      name: this.form.get('name')?.value,
      password: this.form.get('password')?.value
    };

    this.api.post('auth/sign-up', data)
      .subscribe({
        next: () => {
          this.ns.notifySuccess('You are successfully registered');
          this.router.navigate(['/sign-in'], {queryParams: {login: data.username}});
        },
        error: err => {
          this.ns.notifyError(`Registration failed. ${err.error?.message ?? ''}`);
          this.inProgress = false;
        }
      });
  }
}

export class ConfirmPasswordStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    return (!!control?.touched || !!form?.submitted) && !!form?.hasError('passwordMismatch');
  }
}
