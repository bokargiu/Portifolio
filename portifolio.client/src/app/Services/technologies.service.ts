import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { api } from '../../main';

@Injectable({
  providedIn: 'root'
})
export class TechnologiesService {
http = inject(HttpClient);
  constructor() { }

  getTechnologies() {
    return this.http.get(`${api}Technologie`);
  }
}
