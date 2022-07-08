import {Component, OnInit} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {NotificationService} from "../../services/notification.service";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {SignUpData} from "../../interfaces/sign-up-data";
import {ConfirmPasswordStateMatcher} from "../../shared/confirm-password-state-matcher";
import {confirmPasswordValidator} from "../../shared/confirm-password-validator";

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

  form = this.fb.group({
    username: ['', Validators.pattern(/^[a-z\d-._@+]*$/i)],
    email: [''],
    name: ['', Validators.pattern(/^[a-z ]*$/i)],
    password: [''],
    confirmPassword: ['']
  }, {validators: confirmPasswordValidator('password', 'confirmPassword')});

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
      name: this.form.get('name')?.value.trim(),
      password: this.form.get('password')?.value
    };

    if (data.name === '') {
      data.name = undefined;
    }

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
