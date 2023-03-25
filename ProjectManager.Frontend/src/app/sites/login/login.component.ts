import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {CrudService} from "../../services/crud.service";
import {User} from "../../entities/user";
import {Router} from "@angular/router";
import {Language} from "../../entities/language";
import {LangService} from "../../services/lang.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  public form: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.email]),
    password: new FormControl(''),
  });
  public error: string;

  public constructor(public langs: LangService, private crud: CrudService, private router: Router) {
    this.form.reset();
    this.error = "";
  }

  public async submit() {
    this.error = "";
    const email = this.form.get("email").value;
    const password = this.form.get("password").value;
    const user: User = {email: email, password: password};

    const result = await this.crud.sendPostRequest<{token: string}>("users/login", user);
    if (result.success) {
      this.crud.setAuthKey(result.content.token);
      await this.crud.loadUser(true);
      await this.router.navigate(["/dashboard"]);
    }else {
      this.error = this.langs.currentLang.emailOrPasswordWrong;
    }
  }
}
