import {Component, OnInit} from '@angular/core';
import {ProjectService} from "../../services/project.service";
import {CrudService} from "../../services/crud.service";
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {DialogComponent} from "../../components/dialog/dialog.component";
import {firstValueFrom} from "rxjs";
import {MatSnackBar} from "@angular/material/snack-bar";
import {TextDialogComponent} from "../../components/text-dialog/text-dialog.component";
import {NavigationComponent} from "../../components/navigation/navigation.component";
import {Project} from "../../entities/project";
import {LangService} from "../../services/lang.service";
import {Language} from "../../entities/language";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  public constructor(public langs: LangService, public crud: CrudService, public projects: ProjectService, public router: Router, private dialog: MatDialog, private snackBar: MatSnackBar) {}

  public async openProject(projectId: string) {
    const response = await this.crud.sendGetRequest<{url: string}>('projects/' + projectId + '/url/string');
    const url = response.content.url;
    if (!url.startsWith("https")) {
      window.open(`${this.crud.backendUrl}projects/${projectId}/url?token=${this.crud.authKey}`, '_blank').focus();
    } else {
      await this.router.navigate(['/project', projectId]);
    }
  }

  public async editProject(projectId: string) {
    const dialogRef = this.dialog.open(TextDialogComponent, {
      data: {title: "Projekt umbenennen", subtitle: "Name", buttons: [
          {text: "Abbrechen", value: false},
          {text: "Projekt bearbeiten", value: true, color: 'primary'}
        ]}
    });

    const result = await firstValueFrom(dialogRef.afterClosed()) as {success: boolean, data: string};
    if (!result?.success) return;
    await this.projects.editProject(projectId, result.data);
    await this.projects.loadProjects();
    this.snackBar.open("Projekt aktualisiert!", undefined, {duration: 2000});
  }

  public async deleteProject(projectId: string) {
    const dialogRef = this.dialog.open(DialogComponent, {
      data: {title: "Möchtest du das Projekt wirklich löschen?", subtitle: "Alle gespeicherten Daten gehen dann verloren!", buttons: [
          {text: "Abbrechen", value: false},
          {text: "Löschen", value: true, color: 'warn'}
      ]}
    });

    const result = await firstValueFrom(dialogRef.afterClosed());
    if (!result) return;
    NavigationComponent.spinnerVisible = true;
    await this.projects.deleteProject(projectId);
    NavigationComponent.spinnerVisible = false;
    await this.projects.loadProjects();
    this.snackBar.open("Projekt gelöscht!", undefined, {duration: 2000});
  }

  public async updateProjectStatus(projectId: string, start: boolean) {
    if (start) await this.projects.startProject(projectId);
    else await this.projects.stopProject(projectId);
    await this.projects.loadProjects();
  }

}
