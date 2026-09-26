import { Component, inject, OnInit } from '@angular/core';
import { ProjectsService } from '../../Services/projects.service';
import { Router } from '@angular/router';

interface Project {
  id: string;
  name: string;
  description: string;
  url: string;
  type: number;
  start: string;
  end: string;
  iconsTech: string[];
}

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent implements OnInit {

  private proj = inject(ProjectsService);
  private router = inject(Router);
  projects: Project[] = [];

  ngOnInit(): void {
    this.proj.getProjects().subscribe((data: any) => {
      this.projects = data;
    });
  }
  openProject(url: string): void {
    window.open(url);
  }
}
