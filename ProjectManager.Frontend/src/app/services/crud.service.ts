import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {User} from "../entities/user";
import {StorageService} from "./storage.service";

let backend: string;

export interface BackendResponse<T> {
  content: T;
  success: boolean;
  code: number;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CrudService {
  public user: User;
  public onUserUpdate: (() => void)[] = [];
  public authKey: string;
  private headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': ''
  });

  constructor(public client: HttpClient, private storage: StorageService) {
    this.getBackendUrl().then(() => {
      this.authKey = storage.getItem("api_key");
      this.setAuthKey(this.authKey);
    })
  }

  private async getBackendUrl() {
    backend = (await firstValueFrom(this.client.get<{url: string}>(location?.origin + "/backend"))).url;
  }

  public setAuthKey(key: string): void {
    this.authKey = key;
    this.headers = this.headers.set("Authorization", key || '');
    this.storage.setItem("api_key", key);
  }

  public async isAuthenticated(): Promise<boolean> {
    if (this.authKey == undefined) return false;
    const result = await this.sendGetRequest("users/token");
    return result.success;
  }

  public async loadUser(forceLoad: boolean = false): Promise<User> {
    if (this.authKey == undefined) return undefined;
    if (this.user != undefined && !forceLoad) return this.user;
    this.onUserUpdate.forEach(update => update.call(this));
    const response = await this.sendGetRequest<User>("users/me");
    this.user = response.content;
    return this.user;
  }

  public get backendUrl(): string {
    return backend;
  }

  public async sendGetRequest<T>(endpoint: string): Promise<BackendResponse<T>> {
    try {
      const result = await firstValueFrom(this.client.get<T>(backend + endpoint, {headers: this.headers}));
      return {content: result, success: true, code: 200};
    } catch (e) {
      const error = e as HttpErrorResponse;

      if (error.status == 0)
        return {content: undefined, success: false, code: error.status, message: "Server nicht erreichbar!"};

      return {content: undefined, success: false, code: error.status, message: error.error};
    }
  }

  public async sendPutRequest<T>(endpoint: string, body?: any): Promise<BackendResponse<T>> {
    try {
      const result = await firstValueFrom(this.client.put<T>(backend + endpoint, body, {headers: this.headers}));
      return {content: result, success: true, code: 200};
    } catch (e) {
      const error = e as HttpErrorResponse;

      if (error.status == 0)
        return {content: undefined, success: false, code: error.status, message: "Server nicht erreichbar!"};

      return {content: undefined, success: false, code: error.status, message: error.error};
    }
  }

  public async sendPostRequest<T>(endpoint: string, body?: any): Promise<BackendResponse<T>> {
    try {
      const result = await firstValueFrom(this.client.post<T>(backend + endpoint, body, {headers: this.headers}));
      return {content: result, success: true, code: 200};
    } catch (e) {
      const error = e as HttpErrorResponse;

      if (error.status == 0)
        return {content: undefined, success: false, code: error.status, message: "Server nicht erreichbar!"};

      return {content: undefined, success: false, code: error.status, message: error.error};
    }
  }

  public async sendDeleteRequest<T>(endpoint: string): Promise<BackendResponse<T>> {
    try {
      const result = await firstValueFrom(this.client.delete<T>(backend + endpoint, {headers: this.headers}));
      return {content: result, success: true, code: 200};
    } catch (e) {
      const error = e as HttpErrorResponse;

      if (error.status == 0)
        return {content: undefined, success: false, code: error.status, message: "Server nicht erreichbar!"};

      return {content: undefined, success: false, code: error.status, message: error.error};
    }
  }

}
