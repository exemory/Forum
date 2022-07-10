import {Component, OnInit} from '@angular/core';
import {PostWithDetails} from "../../interfaces/post-with-details";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ThreadWithDetails} from "../../interfaces/thread-with-details";
import {AuthService} from "../../services/auth.service";
import {FormBuilder} from "@angular/forms";
import {PostCreationData} from "../../interfaces/post-creation-data";
import {MatDialog} from "@angular/material/dialog";
import {PostUpdateData} from "../../interfaces/post-update-data";
import {EditPostDialogComponent} from "./edit-post-dialog/edit-post-dialog.component";

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {

  loading = true;
  thread!: ThreadWithDetails;
  posts!: PostWithDetails[];

  postForm = this.fb.group({
    content: ['']
  });

  constructor(private route: ActivatedRoute,
              private api: HttpClient,
              private ns: NotificationService,
              private auth: AuthService,
              private fb: FormBuilder,
              private router: Router,
              private dialog: MatDialog) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const threadId = params.get('id')!;

      if (!threadId.match(/^[\da-f]{8}-([\da-f]{4}-){3}[\da-f]{12}$/i)) {
        this.threadDoesNotExists();
        return;
      }

      this.loadThreadInfo(threadId);
    });
  }

  private loadThreadInfo(threadId: string) {
    this.api.get<ThreadWithDetails>(`threads/${threadId}`)
      .subscribe({
        next: thread => {
          this.thread = thread;
          this.loadPosts(this.thread.id);
        },
        error: err => {
          if (err.status === HttpStatusCode.NotFound) {
            this.threadDoesNotExists();
            return;
          }
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
        }
      });
  }

  private threadDoesNotExists() {
    this.ns.notifyError('Thread does not exist');
    this.router.navigate(['../']);
  }

  private loadPosts(threadId: string) {
    this.api.get<PostWithDetails[]>(`posts?threadId=${threadId}`)
      .subscribe({
        next: posts => {
          this.posts = posts;
          this.loading = false
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
        }
      });
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

  isOwnPost(post: PostWithDetails): boolean | undefined {
    if (!this.auth.isLoggedIn) {
      return;
    }

    return post.author?.id === this.auth.session?.userId;
  }

  sendingPost = false;

  onPostSubmit() {
    if (this.postForm.invalid || this.sendingPost) {
      return;
    }

    const data: PostCreationData = {
      content: this.postForm.get('content')?.value?.trim(),
      threadId: this.thread.id
    }

    if (!data.content) {
      return;
    }

    this.sendingPost = true;

    this.api.post<PostWithDetails>('posts', data)
      .subscribe({
        next: post => {
          this.posts.push(post);
          this.ns.notifySuccess("Post has been created");
          this.postForm.reset();
          this.sendingPost = false;
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
          this.sendingPost = false;
        }
      });
  }

  openEditPostDialog(post: PostWithDetails) {
    const dialogRef = this.dialog.open(EditPostDialogComponent,
      {
        maxWidth: '600px',
        width: '100%',
        data: post
      }
    );

    dialogRef.afterClosed().subscribe((data?: PostUpdateData) => {
        if (data !== undefined) {
          post.content = data.content;
        }
      }
    );
  }

  deletePost(post: PostWithDetails) {
    this.api.delete(`posts/${post.id}`)
      .subscribe({
        next: () => {
          const index = this.posts.indexOf(post);
          if (index !== -1) {
            this.posts.splice(index, 1);
          }

          this.ns.notifySuccess("Post has been deleted");
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
