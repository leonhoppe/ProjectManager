import {Component, OnInit} from '@angular/core';
import {CrudService} from "../../services/crud.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {MatDialog} from "@angular/material/dialog";
import {DialogComponent} from "../../components/dialog/dialog.component";
import {firstValueFrom} from "rxjs";
import {User} from "../../entities/user";
import {MatSnackBar} from "@angular/material/snack-bar";
import {Router} from "@angular/router";
import {Language} from "../../entities/language";
import {LangService} from "../../services/lang.service";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent {
  public form: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.email, Validators.maxLength(255)]),
    username: new FormControl('', [Validators.maxLength(255)]),
    password: new FormControl('', [Validators.maxLength(255)]),
    passwordRepeat: new FormControl('', [Validators.maxLength(255)])
  });
  public error: string;

  public constructor(public langs: LangService, public crud: CrudService, private router: Router, public dialog: MatDialog, private snackBar: MatSnackBar) {
    this.form.get("email").setValue(this.crud.user?.email);
    this.form.get("username").setValue(this.crud.user?.username);
  }

  public async update() {
    if (!this.form.valid) return;
    const result = await this.openDialog(this.langs.currentLang?.saveChanges);
    if (!result) return;

    this.error = "";
    const email = this.form.get("email").value;
    const username = this.form.get("username").value;
    const password = this.form.get("password").value;
    const passwordRepeat = this.form.get("passwordRepeat").value;

    if (password != passwordRepeat) {
      this.error = this.langs.currentLang?.passwordsDontMatch;
      return;
    }

    const user: User = {userId: this.crud.user.userId, email, username, password};

    const response = await this.crud.sendPutRequest("users", user);
    if (!response.success) {
      this.error = this.langs.currentLang?.updateFailed;
      return;
    }

    await this.crud.loadUser(true);
    this.form.reset();
    this.snackBar.open(this.langs.currentLang?.updateAccount, undefined, {duration: 2000});
    await this.router.navigate(["dashboard"]);
  }

  public async delete() {
    const result = await this.openDialog(this.langs.currentLang?.deleteQuestion, this.langs.currentLang?.deleteWarning, ['', 'warn']);
    if (!result) return;

    await this.crud.sendDeleteRequest("users");

    this.crud.setAuthKey(undefined);
    this.crud.user = undefined;
    this.snackBar.open(this.langs.currentLang?.accountDeleted, undefined, {duration: 2000});
    await this.router.navigate(["login"]);
  }

  private openDialog(title: string, subtitle?: string, colors?: string[]): Promise<boolean> {
    if (colors == undefined) colors = ['', 'accent'];

    return new Promise<boolean>(async (resolve) => {
      const dialogRef = this.dialog.open(DialogComponent, {
        data: {title, subtitle, buttons: [
            {text: this.langs.currentLang?.cancel, value: false, color: colors[0]},
            {text: this.langs.currentLang?.submit, value: true, color: colors[1]},
          ]}
      });

      resolve(await firstValueFrom(dialogRef.afterClosed()));
    })
  }
}
