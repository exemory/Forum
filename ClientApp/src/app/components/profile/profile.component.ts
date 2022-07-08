import {Component, OnInit} from '@angular/core';
import {UserWithDetails} from "../../interfaces/user-with-details";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {MatDialog} from "@angular/material/dialog";
import {ChangePasswordDialogComponent} from "./change-password-dialog/change-password-dialog.component";
import {EditProfileDialogComponent} from "./edit-profile-dialog/edit-profile-dialog.component";
import {AccountUpdateData} from "../../interfaces/account-update-data";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  loading = true;
  profileInfo!: UserWithDetails;

  constructor(private api: HttpClient,
              private ns: NotificationService,
              private dialog: MatDialog) {
  }

  ngOnInit(): void {
    this.api.get<UserWithDetails>('account')
      .subscribe({
        next: accountInfo => {
          this.profileInfo = accountInfo;
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
        data: this.profileInfo
      });

    dialogRef.afterClosed().subscribe((data?: AccountUpdateData) => {
        if (data !== undefined) {
          this.profileInfo.username = data.username;
          this.profileInfo.email = data.email;
          this.profileInfo.name = data.name;
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
