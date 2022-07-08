import {ErrorStateMatcher} from "@angular/material/core";
import {FormControl, FormGroupDirective, NgForm} from "@angular/forms";

export class ConfirmPasswordStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    return (!!control?.touched || !!form?.submitted) && !!form?.hasError('passwordMismatch');
  }
}
