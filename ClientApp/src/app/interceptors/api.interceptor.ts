import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor, HttpStatusCode
} from '@angular/common/http';
import {Observable, tap} from 'rxjs';
import {AuthService} from "../services/auth.service";
import {environment as env} from "../../environments/environment";
import {Router} from "@angular/router";
import {NotificationService} from "../services/notification.service";

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService,
              private ns: NotificationService,
              private router: Router) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    request = request.clone({url: `${env.apiUrlPrefix}${request.url}`});

    if (this.auth.isLoggedIn) {
      const token = this.auth.token!;

      request = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${token}`)
      });
    }

    return next.handle(request).pipe(
      tap({
          error: err => {
            if (err.status === HttpStatusCode.Unauthorized) {
              this.auth.signOut();
              this.ns.notifyError("Session expired, please sign in again");
              this.router.navigate(['/sign-in']);
            }
          }
        }
      ));
  }
}
