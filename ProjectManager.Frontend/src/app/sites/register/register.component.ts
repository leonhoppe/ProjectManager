import { Component } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {CrudService} from "../../services/crud.service";
import {Router} from "@angular/router";
import {User} from "../../entities/user";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  public form: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.email, Validators.maxLength(255)]),
    username: new FormControl('', [Validators.maxLength(255)]),
    password: new FormControl('', [Validators.maxLength(255)]),
    passwordRepeat: new FormControl('', [Validators.maxLength(255)])
  });
  public error: string;

  public constructor(private crud: CrudService, private router: Router) {}

  public async submit() {
    this.error = "";
    const email = this.form.get("email").value;
    const username = this.form.get("username").value;
    const password = this.form.get("password").value;
    const passwordRepeat = this.form.get("passwordRepeat").value;

    if (password != passwordRepeat) {
      this.error = "Passwörter stimmen nicht überein";
      return;
    }

    const user: User = {email, username, password};
    const result = await this.crud.sendPostRequest<{token: string}>("users/register", user);
    if (!result.success) {
      this.error = "Registrierung fehlgeschlagen";
      return;
    }

    this.crud.setAuthKey(result.content.token);
    await this.crud.loadUser(true);
    await this.router.navigate(["/dashboard"]);
  }
}
