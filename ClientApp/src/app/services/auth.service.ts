import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {tap} from "rxjs";
import {Session} from "../interfaces/session";
import {JwtHelperService} from "@auth0/angular-jwt";
import {NotificationService} from "./notification.service";
import {SignInData} from "../interfaces/singInData";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private jwtHelper = new JwtHelperService();

  constructor(private api: HttpClient, private ns: NotificationService) {
    if (this.isLoggedIn && this.jwtHelper.isTokenExpired(this.session?.token)) {
      this.signOut();
    }
  }

  public signIn(login: string, password: string) {
    return this.api.post<Session>('auth/sign-in', <SignInData>{login, password})
      .pipe(
        tap(this.setSession)
      );
  }

  public signOut() {
    localStorage.removeItem('session');
  }

  private setSession(session: Session) {
    localStorage.setItem('session', JSON.stringify(session));
  }

  get isLoggedIn(): boolean {
    return this.session !== undefined;
  }

  get session(): Session | undefined {
    const session = localStorage.getItem('session');

    if (session == null) {
      return;
    }

    return JSON.parse(session);
  }
}
