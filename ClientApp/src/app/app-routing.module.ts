import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {SignInComponent} from "./components/sign-in/sign-in.component";
import {SignUpComponent} from "./components/sign-up/sign-up.component";
import {ThreadListComponent} from "./components/thread-list/thread-list.component";
import {PostListComponent} from "./components/post-list/post-list.component";
import {UsersComponent} from "./components/users/users.component";

const routes: Routes = [
  {path: 'threads', component: ThreadListComponent},
  {path: 'threads/:id', component: PostListComponent},
  {path: 'sign-in', component: SignInComponent},
  {path: 'sign-up', component: SignUpComponent},
  {path: 'users', component: UsersComponent},
  {path: '**', redirectTo: 'threads'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
