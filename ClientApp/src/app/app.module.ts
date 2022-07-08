import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {MatCardModule} from "@angular/material/card";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatIconModule} from "@angular/material/icon";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import {SignInComponent} from "./components/sign-in/sign-in.component";
import {SignUpComponent} from "./components/sign-up/sign-up.component";
import {ApiInterceptor} from "./interceptors/api.interceptor";
import {NavbarComponent} from './components/navbar/navbar.component';
import {MatMenuModule} from "@angular/material/menu";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {MatDialogModule} from "@angular/material/dialog";
import {ThreadsComponent} from "./components/threads/threads.component";
import {NewThreadDialogComponent} from "./components/threads/new-thread-dialog/new-thread-dialog.component";
import {PostsComponent} from './components/posts/posts.component';
import {UsersComponent} from './components/users/users.component';
import {MatTableModule} from "@angular/material/table";
import {MatPaginatorModule} from "@angular/material/paginator";
import {MatSortModule} from "@angular/material/sort";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatNativeDateModule} from "@angular/material/core";
import { EditThreadDialogComponent } from './components/threads/edit-thread-dialog/edit-thread-dialog.component';
import { EditPostDialogComponent } from './components/posts/edit-post-dialog/edit-post-dialog.component';
import { ProfileComponent } from './components/profile/profile.component';
import { EditProfileDialogComponent } from './components/profile/edit-profile-dialog/edit-profile-dialog.component';
import { ChangePasswordDialogComponent } from './components/profile/change-password-dialog/change-password-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    SignUpComponent,
    NavbarComponent,
    ThreadsComponent,
    NewThreadDialogComponent,
    PostsComponent,
    UsersComponent,
    EditThreadDialogComponent,
    EditPostDialogComponent,
    ProfileComponent,
    EditProfileDialogComponent,
    ChangePasswordDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatSnackBarModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
