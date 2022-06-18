import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {MatPaginator} from "@angular/material/paginator";
import {MatTableDataSource} from "@angular/material/table";
import {UserWithDetails} from "../../interfaces/user-with-details";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {MatSort} from "@angular/material/sort";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, AfterViewInit {

  loading = true;
  displayedColumns = ['username', 'email', 'name', 'registrationDate', 'roles', 'options'];
  dataSource = new MatTableDataSource<UserWithDetails>();
  filterValue = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private api: HttpClient,
              private ns: NotificationService,
              private auth: AuthService) {
  }

  ngOnInit(): void {
    this.api.get<UserWithDetails[]>('users')
      .subscribe({
        next: users => {
          this.dataSource.data = users;
          this.loading = false;
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. Error ${err.status}`, true);
        }
      });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter() {
    this.dataSource.filter = this.filterValue;

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  clearFilter() {
    this.filterValue = '';
    this.applyFilter();
  }

  isModerator(user: UserWithDetails): boolean {
    return user.roles.includes('Moderator');
  }

  isAdmin(user: UserWithDetails): boolean {
    return user.roles.includes('Administrator');
  }

  isSelf(user: UserWithDetails): boolean {
    return this.auth.session?.username === user.username;
  }

  promoteToModerator(user: UserWithDetails) {
    this.api.put(`users/${user.id}/promote-to-moderator`, undefined)
      .subscribe({
        next: () => {
          user.roles = ['Moderator'];
          this.ns.notifySuccess(`User '${user.username}' has been promoted to Moderator`);
        },
        error: err => {
          this.ns.notifyError(`Operation failed. Error ${err.status}`);
        }
      });
  }

  demoteToUser(user: UserWithDetails) {
    this.api.put(`users/${user.id}/demote-to-user`, undefined)
      .subscribe({
        next: () => {
          user.roles = ['User'];
          this.ns.notifySuccess(`User '${user.username}' has been demoted to User`);
        },
        error: err => {
          this.ns.notifyError(`Operation failed. Error ${err.status}`);
        }
      });
  }

  deleteUser(user: UserWithDetails) {
    this.api.delete(`users/${user.id}`)
      .subscribe({
        next: () => {
          const index = this.dataSource.data.indexOf(user);
          if (index !== -1) {
            this.dataSource.data.splice(index, 1);
            this.dataSource.data = this.dataSource.data;
          }
          this.ns.notifySuccess(`User '${user.username}' has been deleted`);
        },
        error: err => {
          this.ns.notifyError(`Operation failed. Error ${err.status}`);
        }
      });
  }
}
