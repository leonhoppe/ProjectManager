import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from "./sites/login/login.component";
import {RegisterComponent} from "./sites/register/register.component";
import {ProfileComponent} from "./sites/profile/profile.component";
import {DashboardComponent} from "./sites/dashboard/dashboard.component";
import {ProjectComponent} from "./sites/project/project.component";

const routes: Routes = [
  {path: "login", component: LoginComponent},
  {path: "register", component: RegisterComponent},
  {path: "dashboard", component: DashboardComponent},
  {path: "profile", component: ProfileComponent},
  {path: "project/:id", component: ProjectComponent},
  {path: "**", pathMatch: "full", redirectTo: "/dashboard"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    initialNavigation: 'enabledBlocking'
})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
