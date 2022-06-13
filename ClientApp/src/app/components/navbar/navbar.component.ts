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
    return this.auth.session?.username!;
  }

  get isUserAdmin(): boolean | undefined {
    if (!this.auth.isLoggedIn) {
      return undefined;
    }

    return this.auth.session?.roles.includes("Administrator");
  }

  get isUserModerator(): boolean | undefined {
    if (!this.auth.isLoggedIn) {
      return undefined;
    }

    return this.auth.session?.roles.includes("Moderator");
  }

  signOut() {
    this.auth.signOut();
  }
}
