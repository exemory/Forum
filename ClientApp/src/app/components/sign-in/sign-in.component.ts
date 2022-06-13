import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {FormBuilder} from "@angular/forms";
import {NotificationService} from "../../services/notification.service";
import {HttpStatusCode} from "@angular/common/http";
import {Router} from "@angular/router";

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent implements OnInit {

  form = this.fb.group({
    login: [''],
    password: ['']
  });

  hidePassword = true;
  inProgress = false;

  constructor(private auth: AuthService,
              private fb: FormBuilder,
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

    this.auth.signIn(this.form.value.login, this.form.value.password)
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: err => {
          this.inProgress = false;
          this.ns.notifyError(err.status === HttpStatusCode.Unauthorized ?
            "Login or password is incorrect" : `Authentication failed. Error ${err.status}`);
        }
      });
  }
}
