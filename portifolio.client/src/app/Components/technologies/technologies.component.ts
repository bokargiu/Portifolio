import { Component, inject, OnInit } from '@angular/core';
import { TechnologiesService } from '../../Services/technologies.service';

interface IconInfo {
  id: string;
  icon: string;
  title: string;
  infos: string;
  show: boolean;
  x?: number;
  y?: number;
  expandTo?: 'left-up' | 'left-down' | 'right-up' | 'right-down' | 'center-up' | 'center-down';
}

@Component({
  selector: 'app-technologies',
  standalone: true,
  imports: [],
  templateUrl: './technologies.component.html',
  styleUrl: './technologies.component.scss'
})
export class TechnologiesComponent implements OnInit {

  private tech = inject(TechnologiesService);
  tecnologies: IconInfo[] = [];

  ngOnInit(): void {
    this.tech.getTechnologies().subscribe((data: any) => {
      this.tecnologies = data;
    });
  }

  expandeTech(event: MouseEvent, technologie: IconInfo, index: number) {
    const icon = event.currentTarget as HTMLElement;
    const container = icon.parentElement as HTMLElement;

    const containerRect = container.getBoundingClientRect();

    let up = false;
    if (icon.getBoundingClientRect().y > window.innerHeight / 2) {
      up = true;
    }

    switch (index % 5) {
      case 0:
      case 1:
        technologie.expandTo = up ? 'right-up' : 'right-down';
        technologie.x = containerRect.left;
        break;

      case 2:
        technologie.expandTo = up ? 'center-up' : 'center-down';
        technologie.x = containerRect.left + containerRect.width / 2;
        break;

      default:
        technologie.expandTo = up ? 'left-up' : 'left-down';
        technologie.x = containerRect.right;
        break;
    }

    technologie.y = up
      ? containerRect.top + window.scrollY
      : containerRect.top + window.scrollY + containerRect.height;
    technologie.show = true;
  }
}
