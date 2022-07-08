import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {SignInComponent} from "./components/sign-in/sign-in.component";
import {SignUpComponent} from "./components/sign-up/sign-up.component";
import {ThreadsComponent} from "./components/threads/threads.component";
import {PostsComponent} from "./components/posts/posts.component";
import {UsersComponent} from "./components/users/users.component";
import {GuestsGuard} from "./guards/guests.guard";
import {AdminsGuard} from "./guards/admins.guard";
import {ProfileComponent} from "./components/profile/profile.component";
import {AuthorizedUsersGuard} from "./guards/authorized-users.guard";

const routes: Routes = [
  {path: 'threads', component: ThreadsComponent},
  {path: 'threads/:id', component: PostsComponent},
  {path: 'sign-in', component: SignInComponent, canActivate: [GuestsGuard]},
  {path: 'sign-up', component: SignUpComponent, canActivate: [GuestsGuard]},
  {path: 'users', component: UsersComponent, canActivate: [AdminsGuard]},
  {path: 'profile', component: ProfileComponent, canActivate: [AuthorizedUsersGuard]},
  {path: '**', redirectTo: 'threads'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
