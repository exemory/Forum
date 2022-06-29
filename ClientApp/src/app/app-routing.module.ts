import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {SignInComponent} from "./components/sign-in/sign-in.component";
import {SignUpComponent} from "./components/sign-up/sign-up.component";
import {ThreadsComponent} from "./components/threads/threads.component";
import {PostsComponent} from "./components/posts/posts.component";
import {UsersComponent} from "./components/users/users.component";
import {OnlyGuestsGuard} from "./guards/only-guests.guard";
import {OnlyAdminsGuard} from "./guards/only-admins.guard";

const routes: Routes = [
  {path: 'threads', component: ThreadsComponent},
  {path: 'threads/:id', component: PostsComponent},
  {path: 'sign-in', component: SignInComponent, canActivate: [OnlyGuestsGuard]},
  {path: 'sign-up', component: SignUpComponent, canActivate: [OnlyGuestsGuard]},
  {path: 'users', component: UsersComponent, canActivate: [OnlyAdminsGuard]},
  {path: '**', redirectTo: 'threads'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
