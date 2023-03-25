import { Injectable } from '@angular/core';
import {CrudService} from "./crud.service";
import {Project} from "../entities/project";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  public projects: Project[] = []

  constructor(private crud: CrudService) {
    crud.onUserUpdate.push(this.loadProjects.bind(this));
  }

  public async loadProjects() {
    this.projects = [];
    const result = (await this.crud.sendGetRequest<{projects: Project[], running: boolean[]}>("projects")).content;
    for (let i = 0; i < result.projects.length; i++) {
      this.projects[i] = result.projects[i];
      this.projects[i].running = result.running[i];
    }
  }

  public async getProject(projectId: string): Promise<Project> {
    const response = await this.crud.sendGetRequest<Project>("projects/" + projectId);
    return response.content;
  }

  public async addProject(name: string): Promise<string> {
    const response = await this.crud.sendPostRequest<{projectId: string}>("projects", {name});
    return response.content?.projectId;
  }

  public async editProject(projectId: string, name: string, domain: string): Promise<boolean> {
    const response = await this.crud.sendPutRequest("projects/" + projectId, {name, domain});
    return response.success;
  }

  public async deleteProject(projectId: string): Promise<boolean> {
    const response = await this.crud.sendDeleteRequest("projects/" + projectId);
    return response.success;
  }

  public async startProject(projectId: string): Promise<boolean> {
    const response = await this.crud.sendGetRequest("projects/" + projectId + "/start");
    return response.success;
  }

  public async stopProject(projectId: string): Promise<boolean> {
    const response = await this.crud.sendGetRequest("projects/" + projectId + "/stop");
    return response.success;
  }

  public async isProjectRunning(projectId: string): Promise<boolean> {
    const response = await this.crud.sendGetRequest<{started: boolean}>("projects/" + projectId + "/status");
    return response.content?.started;
  }
}
