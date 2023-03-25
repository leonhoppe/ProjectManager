import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {CrudService} from "../../services/crud.service";
import {ProjectService} from "../../services/project.service";
import {MatDialog} from "@angular/material/dialog";
import {TextDialogComponent} from "../text-dialog/text-dialog.component";
import {firstValueFrom} from "rxjs";
import {MatSnackBar} from "@angular/material/snack-bar";
import {StorageService} from "../../services/storage.service";
import {LangService} from "../../services/lang.service";
import {Language} from "../../entities/language";

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent {
  public static spinnerVisible: boolean = false;
  public darkMode: boolean;

  public constructor(public router: Router, public langs: LangService, public crud: CrudService, public projects: ProjectService, public dialog: MatDialog, private snackBar: MatSnackBar, private storage: StorageService) {
    this.darkMode = storage.getItem("darkMode") == "true";
  }

  public isSpinnerVisible(): boolean {
    return NavigationComponent.spinnerVisible;
  }

  public onModeChange(): void {
    this.darkMode = !document.body.classList.contains("darkMode");
    document.body.classList.toggle("darkMode", this.darkMode);
    this.storage.setItem("darkMode", JSON.stringify(this.darkMode));
  }

  public showActions(): boolean {
    return this.router.url != '/' && this.router.url != '/login' && this.router.url != '/register';
  }

  public async logout() {
    this.crud.setAuthKey(undefined);
    this.crud.user = undefined;
    await this.router.navigate(["login"]);
  }

  public async createProject() {
    const dialogRef = this.dialog.open(TextDialogComponent, {
      data: {title: "Neues Projekt", subtitle: "Name", buttons: [
          {text: "Abbrechen", value: false},
          {text: "Projekt erstellen", value: true, color: 'primary'}
      ]}
    });

    const result = await firstValueFrom(dialogRef.afterClosed()) as {success: boolean, data: string};
    if (!result?.success) return;
    NavigationComponent.spinnerVisible = true;
    const projectId = await this.projects.addProject(result.data);
    NavigationComponent.spinnerVisible = false;
    if (projectId == undefined) {
      this.snackBar.open("Projekt kann nicht erstellt werden!", undefined, {duration: 2000});
      return;
    }
    await this.projects.loadProjects();
    this.snackBar.open("Projekt erstellt!", undefined, {duration: 2000});
  }

  public openProject(projectId: string) {
    window.open(`${this.crud.backendUrl}projects/${projectId}/url?token=${this.crud.authKey}`, '_blank').focus();
  }
}
