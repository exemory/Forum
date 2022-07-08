import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AuthService} from "../../services/auth.service";
import {MatDialog} from "@angular/material/dialog";
import {NewThreadDialogComponent} from "./new-thread-dialog/new-thread-dialog.component";
import {ThreadWithDetails} from "../../interfaces/thread-with-details";
import {NotificationService} from "../../services/notification.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ThreadStatusUpdateData} from "../../interfaces/thread-status-update-data";
import {EditThreadDialogComponent} from "./edit-thread-dialog/edit-thread-dialog.component";
import {ThreadUpdateData} from "../../interfaces/thread-update-data";

@Component({
  selector: 'app-threads',
  templateUrl: './threads.component.html',
  styleUrls: ['./threads.component.scss']
})
export class ThreadsComponent implements OnInit {

  loading = true;
  threads!: ThreadWithDetails[];

  filterValue = '';
  fromDate?: Date;
  toDate?: Date;

  constructor(private api: HttpClient,
              private auth: AuthService,
              private dialog: MatDialog,
              private ns: NotificationService,
              private router: Router,
              private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.api.get<ThreadWithDetails[]>('threads')
      .subscribe({
        next: threads => {
          this.threads = threads;
          this.loading = false;
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
        }
      })
  }

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  get isUserModeratorOrAdmin(): boolean | undefined {
    if (!this.auth.isLoggedIn) {
      return;
    }

    const roles = this.auth.session?.userRoles;

    return roles?.includes('Moderator') ||
      roles?.includes('Administrator');
  }

  get filteredThreads(): ThreadWithDetails[] {
    let filteredThreads = this.threads;

    if (this.filterValue) {
      filteredThreads = filteredThreads.filter(t => t.topic.toLowerCase().includes(this.filterValue.trim().toLowerCase()));
    }

    if (this.fromDate) {
      filteredThreads = filteredThreads.filter(t => this.getThreadCreationDate(t) >= this.fromDate!);
    }

    if (this.toDate) {
      filteredThreads = filteredThreads.filter(t => this.getThreadCreationDate(t) <= this.toDate!);
    }

    return filteredThreads;
  }

  private getThreadCreationDate(thread: ThreadWithDetails): Date {
    let threadCreationDate = new Date(thread.creationDate);
    threadCreationDate.setHours(0, 0, 0, 0);
    return threadCreationDate;
  }

  openNewThreadDialog() {
    const dialogRef = this.dialog.open(NewThreadDialogComponent,
      {
        maxWidth: '600px',
        width: '100%'
      });

    dialogRef.afterClosed().subscribe((thread?: ThreadWithDetails) => {
        if (thread !== undefined) {
          this.threads.unshift(thread);
          this.router.navigate([thread.id], {relativeTo: this.route});
        }
      }
    );
  }

  openEditThreadDialog(thread: ThreadWithDetails) {
    const dialogRef = this.dialog.open(EditThreadDialogComponent,
      {
        maxWidth: '600px',
        width: '100%',
        data: thread
      }
    );

    dialogRef.afterClosed().subscribe((data?: ThreadUpdateData) => {
        if (data !== undefined) {
          thread.topic = data.topic;
        }
      }
    );
  }

  updateThreadStatus(thread: ThreadWithDetails, closed: boolean) {
    this.api.put(`threads/${thread.id}/status`, <ThreadStatusUpdateData>{closed})
      .subscribe(
        {
          next: () => {
            thread.closed = closed;
            this.ns.notifySuccess(`Thread has been ${closed ? 'closed' : 'opened'}`);
          },
          error: err => {
            this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
          }
        }
      );
  }

  deleteThread(thread: ThreadWithDetails) {
    this.api.delete(`threads/${thread.id}`, undefined)
      .subscribe(
        {
          next: () => {
            const index = this.threads.indexOf(thread);
            if (index !== -1) {
              this.threads.splice(index, 1);
            }

            this.ns.notifySuccess("Thread has been deleted");
          },
          error: err => {
            this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
          }
        }
      );
  }
}
