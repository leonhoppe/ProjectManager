import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  public setItem(setItem,data){
    if (typeof localStorage === "undefined") return;
    localStorage.setItem(setItem, data);
  }

  public getItem(getItem){
    if (typeof localStorage === "undefined") return undefined;
    return localStorage.getItem(getItem);
  }
}
