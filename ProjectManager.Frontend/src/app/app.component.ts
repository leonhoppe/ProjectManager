import { Component } from '@angular/core';
import {CrudService} from "./services/crud.service";
import {Router} from "@angular/router";
import {ProjectService} from "./services/project.service";
import {StorageService} from "./services/storage.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent{

  public constructor(public crud: CrudService, private router: Router, private projects: ProjectService, private storage: StorageService/* ProjectService gets dependency injected because the onUserUpdate listener needs to be created */) {
    if (typeof document === 'undefined') return;
    document.body.classList.toggle("darkMode", storage.getItem("darkMode") == "true");
    setTimeout(this.validateToken.bind(this), 0);
  }

  public isLoginRoute(): boolean {
    return (this.router.url == '/login' || this.router.url == '/register');
  }

  private async validateToken() {
    while (this.crud.backendUrl == undefined) {
      await this.timeout(200);
    }

    if (this.isLoginRoute()) return;
    if (!await this.crud.isAuthenticated())
      await this.router.navigate(["/login"]);
    else await this.crud.loadUser();
  }

  private timeout(ms: number): Promise<void> {
    return new Promise((resolve, reject) => {
      setTimeout(resolve, ms);
    });
  }

}
