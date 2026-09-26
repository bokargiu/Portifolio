import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../Components/header/header.component';
import { HeroComponent } from '../../Components/hero/hero.component';
import { TechnologiesComponent } from '../../Components/technologies/technologies.component';
import { ProjectsComponent } from '../../Components/projects/projects.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    HeaderComponent,
    HeroComponent,
    TechnologiesComponent,
    ProjectsComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  headerHidden = false;
  private lastPositionMouse = 0;

  @HostListener('window:scroll')
  onScroll(): void {
    const currentScrollPosition = window.scrollY;

    if (currentScrollPosition > this.lastPositionMouse) {
      this.headerHidden = true;
    } else {
      this.headerHidden = false;
    }

    this.lastPositionMouse = currentScrollPosition;
  }
}
