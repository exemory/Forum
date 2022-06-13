import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {tap} from "rxjs";
import {SessionDto} from "../interfaces/sessionDto";
import {SessionInfo} from "../interfaces/session-info";
import {JwtHelperService} from "@auth0/angular-jwt";
import {SignInDto} from "../interfaces/singInDto";
import {NotificationService} from "./notification.service";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private jwtHelper = new JwtHelperService();

  constructor(private api: HttpClient, private ns: NotificationService) {
    if (this.isLoggedIn && this.jwtHelper.isTokenExpired(this.token!)) {
      ns.notifyError("Session expired, please sign in again");
      this.signOut();
    }
  }

  public signIn(login: string, password: string) {
    return this.api.post<SessionDto>('auth/sign-in', <SignInDto>{login, password})
      .pipe(
        tap(this.setSession)
      );
  }

  public signOut() {
    if (!this.isLoggedIn) return;
    localStorage.removeItem('token');
  }

  private setSession(token: SessionDto) {
    localStorage.setItem('token', token.token);
  }

  get isLoggedIn(): boolean {
    return this.token !== null;
  }

  get token(): string | null {
    return localStorage.getItem('token');
  }

  get sessionInfo(): SessionInfo | undefined {
    if (!this.isLoggedIn) {
      return;
    }

    const decodedToken = this.jwtHelper.decodeToken(this.token!);

    let roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    if (typeof roles === 'string') {
      roles = [roles];
    }

    return {
      expires: decodedToken['exp'],
      username: decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      userRoles: roles
    };
  }
}
