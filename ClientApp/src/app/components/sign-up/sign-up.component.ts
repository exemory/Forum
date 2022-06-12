import {Component, OnInit} from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {NotificationService} from "../../services/notification.service";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {SignUpDto} from "../../interfaces/signUpDto";

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

  form = this.fb.group({
    username: [''],
    email: [''],
    name: [''],
    password: ['']
  });

  hidePassword = true;
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

    let data: SignUpDto = {
      username: this.form.get('username')?.value,
      email: this.form.get('email')?.value,
      name: this.form.get('name')?.value,
      password: this.form.get('password')?.value,
    }

    this.api.post('auth/sign-up', data)
      .subscribe({
        next: () => {
          this.ns.notifySuccess('You are successfully registered');
          this.router.navigate(['/sign-in']);
        },
        error: err => {
          this.ns.notifyError(`Registration failed. Error ${err.status}`);
          this.inProgress = false;
        }
      })
  }
}
