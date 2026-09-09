import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { api } from '../../main';

@Injectable({
  providedIn: 'root'
})
export class ProjectsService {
http = inject(HttpClient);
  constructor() { }

  getProjects() {
    return this.http.get(`${api}Project`);
  }
}
