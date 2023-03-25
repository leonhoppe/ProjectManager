import { Injectable } from '@angular/core';
import {Language} from "../entities/language";
import {CrudService} from "./crud.service";
import {firstValueFrom} from "rxjs";
import {StorageService} from "./storage.service";

@Injectable({
  providedIn: 'root'
})
export class LangService {
  private languages: Map<string, Language> = new Map<string, Language>();
  public allLanguages: string[] = [];
  public currentLang: Language;

  constructor(private crud: CrudService, private storage: StorageService) {
    this.loadLanguages();
  }

  private async loadLanguages() {
    this.languages = new Map<string, Language>();
    const res = await firstValueFrom(this.crud.client.get<{files: string[]}>(location?.origin + "/lang"));
    const languages = res.files;
    this.allLanguages = languages.map(lang => lang.replace(".json", ""));
    this.currentLang = await this.loadLanguage(this.storage.getItem("language") || "en-US");
  }

  private async loadLanguage(name: string): Promise<Language> {
    const lang = await firstValueFrom(this.crud.client.get<Language>(`${location.origin}/assets/languages/${name}.json`));
    this.languages.set(name, lang);
    return lang;
  }

  public async setLanguage(lang: string) {
    if (this.languages.get(lang) == undefined)
      await this.loadLanguage(lang);

    this.currentLang = this.languages.get(lang);
    this.storage.setItem("language", lang);
  }

}
