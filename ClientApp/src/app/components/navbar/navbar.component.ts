import {ChangeDetectorRef, Component, OnInit, ViewEncapsulation} from '@angular/core';
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class NavbarComponent implements OnInit {

  hideMobileMenu = true;

  constructor(private auth: AuthService,
              private cdr: ChangeDetectorRef) {
  }

  ngOnInit(): void {
    document.addEventListener('click', this.onPageClick.bind(this));
  }

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  get userName(): string {
    return this.auth.session?.username!;
  }

  get isAdmin(): boolean | undefined {
    return this.auth.session?.userRoles.includes("Administrator");
  }

  signOut() {
    this.auth.signOut();
  }

  private onPageClick(e: any)  {
    if (!e.target.closest('.menu-toggle-btn'))
    {
      this.hideMobileMenu = true;
      this.cdr.detectChanges();
    }
  }
}
