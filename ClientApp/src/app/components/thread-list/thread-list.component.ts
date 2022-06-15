import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AuthService} from "../../services/auth.service";
import {MatDialog} from "@angular/material/dialog";
import {NewThreadDialogComponent} from "./new-thread-dialog/new-thread-dialog.component";
import {ThreadWithDetails} from "../../interfaces/thread-with-details";
import {ThreadCreationData} from "../../interfaces/thread-creation-data";
import {NotificationService} from "../../services/notification.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-thread-list',
  templateUrl: './thread-list.component.html',
  styleUrls: ['./thread-list.component.scss']
})
export class ThreadListComponent implements OnInit {

  loaded = false;
  threads!: ThreadWithDetails[];

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
          this.loaded = true;
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

    const roles = this.auth.session?.roles;

    return roles?.includes('Moderator') ||
      roles?.includes('Administrator');
  }

  openNewThreadDialog() {
    const dialogRef = this.dialog.open(NewThreadDialogComponent,
      {
        maxWidth: '600px',
        width: '100%'
      });

    dialogRef.afterClosed().subscribe(result => {
        if (result !== undefined) {
          this.createNewThread(result);
        }
      }
    );
  }

  createNewThread(topic: string) {
    topic = topic.trim();

    this.api.post<ThreadWithDetails>('threads', <ThreadCreationData>{topic})
      .subscribe({
        next: thread => {
          this.threads.unshift(thread);
          this.ns.notifySuccess("New thread has been successfully created");
          this.router.navigate([thread.id], {relativeTo: this.route});
        },
        error: err => {
          this.ns.notifyError(`Failed to create a thread. Error ${err.status}`);
        }
      });
  }

  closeThread(thread: ThreadWithDetails) {
    this.api.put(`threads/${thread.id}/close`, undefined)
      .subscribe(
        {
          next: () => {
            thread.closed = true;
            this.ns.notifySuccess("Thread has been closed");
          },
          error: err => {
            this.ns.notifyError(`Thread closing failed. Error ${err.status}`);
          }
        }
      );
  }

  openThread(thread: ThreadWithDetails) {
    this.api.put(`threads/${thread.id}/open`, undefined)
      .subscribe(
        {
          next: () => {
            thread.closed = false;
            this.ns.notifySuccess("Thread has been opened");
          },
          error: err => {
            this.ns.notifyError(`Thread opening failed. Error ${err.status}`);
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
            this.ns.notifyError(`Thread deletion failed. Error ${err.status}`);
          }
        }
      );
  }
}