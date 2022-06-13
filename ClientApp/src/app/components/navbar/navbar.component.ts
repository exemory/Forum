import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class NavbarComponent implements OnInit {

  constructor(private auth: AuthService) {
  }

  ngOnInit(): void {
  }

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  get userName(): string {
    return this.auth.sessionInfo?.username!;
  }

  get isUserAdmin(): boolean | undefined {
    if (!this.isLoggedIn) return undefined;

    const userRoles = this.auth.sessionInfo?.userRoles;

    return userRoles?.includes("Administrator");
  }

  get isUserModerator(): boolean | undefined {
    if (!this.isLoggedIn) return undefined;

    const userRoles = this.auth.sessionInfo?.userRoles;

    return userRoles?.includes("Moderator");
  }

  signOut() {
    this.auth.signOut();
  }
}
