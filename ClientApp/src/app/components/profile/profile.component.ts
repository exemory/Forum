import {Component, OnInit} from '@angular/core';
import {UserWithDetails} from "../../interfaces/user-with-details";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {MatDialog} from "@angular/material/dialog";
import {ChangePasswordDialogComponent} from "./change-password-dialog/change-password-dialog.component";
import {EditProfileDialogComponent} from "./edit-profile-dialog/edit-profile-dialog.component";
import {AccountUpdateData} from "../../interfaces/account-update-data";
import {ActivatedRoute, Router} from "@angular/router";
import {UserProfileInfo} from "../../interfaces/user-profile-info";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  loading = true;
  profileInfo?: UserProfileInfo;
  ownProfileInfo?: UserWithDetails;

  constructor(private api: HttpClient,
              private ns: NotificationService,
              private dialog: MatDialog,
              private route: ActivatedRoute,
              private router: Router) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const username = params.get('username');
      username ? this.loadProfile(username) : this.loadOwnProfile();
    });
  }

  loadProfile(username: string) {
    this.api.get<UserProfileInfo>(`users/${username}`)
      .subscribe({
        next: profileInfo => {
          this.profileInfo = profileInfo;
          this.loading = false;
        },
        error: err => {
          if (err.status === HttpStatusCode.NotFound) {
            this.ns.notifyError('User does not exist');
            this.router.navigate(['/']);
            return;
          }

          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
        }
      });
  }

  loadOwnProfile() {
    this.api.get<UserWithDetails>('account')
      .subscribe({
        next: profileInfo => {
          this.ownProfileInfo = profileInfo;
          this.loading = false;
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
        }
      });
  }

  openEditProfileDialog() {
    const dialogRef = this.dialog.open(EditProfileDialogComponent,
      {
        maxWidth: '400px',
        width: '100%',
        data: this.ownProfileInfo
      });

    dialogRef.afterClosed().subscribe((data?: AccountUpdateData) => {
        if (data !== undefined) {
          this.ownProfileInfo!.username = data.username;
          this.ownProfileInfo!.email = data.email;
          this.ownProfileInfo!.name = data.name;
        }
      }
    );
  }

  openChangePasswordDialog() {
    this.dialog.open(ChangePasswordDialogComponent,
      {
        maxWidth: '400px',
        width: '100%'
      });
  }
}
