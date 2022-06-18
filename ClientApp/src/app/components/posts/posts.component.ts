import {Component, OnInit} from '@angular/core';
import {PostWithDetails} from "../../interfaces/post-with-details";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ThreadWithDetails} from "../../interfaces/thread-with-details";
import {AuthService} from "../../services/auth.service";
import {FormBuilder} from "@angular/forms";
import {PostCreationData} from "../../interfaces/post-creation-data";

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
    postText: ['']
  });

  constructor(private route: ActivatedRoute,
              private api: HttpClient,
              private ns: NotificationService,
              private auth: AuthService,
              private fb: FormBuilder,
              private router: Router) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const threadId = params.get('id')!;
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
            this.ns.notifyError('Thread does not exist anymore');
            this.router.navigate(['../']);
            return;
          }
          this.ns.notifyError(`Loading data failed. Error ${err.status}`, true);
        }
      });
  }

  private loadPosts(threadId: string) {
    this.api.get<PostWithDetails[]>(`posts?threadId=${threadId}`)
      .subscribe({
        next: posts => {
          this.posts = posts;
          this.loading = false
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. Error ${err.status}`, true);
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

    const roles = this.auth.session?.roles;

    return roles?.includes('Moderator') ||
      roles?.includes('Administrator');
  }

  inProgress = false;

  onPostSubmit() {
    if (this.postForm.invalid) {
      return;
    }

    const data: PostCreationData = {
      content: this.postForm.get('postText')?.value,
      threadId: this.thread.id
    }

    if (data.content == null || data.content === '') {
      return;
    }

    this.api.post<PostWithDetails>('posts', data)
      .subscribe({
        next: post => {
          this.posts.push(post);
          this.ns.notifySuccess("Reply has been sent");
          this.postForm.reset();
          this.inProgress = false;
        },
        error: err => {
          this.ns.notifyError(`Sending reply failed. Error ${err.status}`);
          this.inProgress = false;
        }
      });
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
          this.ns.notifyError(`Post deletion failed. Error ${err.status}`);
        }
      });
  }
}
