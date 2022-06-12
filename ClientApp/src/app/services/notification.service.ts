import {Injectable} from '@angular/core';
import {MatSnackBar, MatSnackBarConfig} from "@angular/material/snack-bar";

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private snackBar: MatSnackBar) {
  }

  notify(message: string, messageType: 'success' | 'error') {

    let config: MatSnackBarConfig = {
      duration: 2500,
      horizontalPosition: "right",
      verticalPosition: "top",
      panelClass: messageType
    }

    return this.snackBar.open(message, undefined, config);
  }

  notifySuccess(message: string) {
    return this.notify(message, 'success');
  }

  notifyError(message: string) {
    return this.notify(message, 'error');
  }
}
