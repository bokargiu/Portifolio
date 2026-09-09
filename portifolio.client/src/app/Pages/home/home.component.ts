import { AfterViewInit, Component, ElementRef, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { HeaderComponent } from '../../Components/header/header.component';
import {MatIconModule} from '@angular/material/icon';
import * as THREE from 'three'
import { NgModel } from '@angular/forms';
import { TechnologiesService } from '../../Services/technologies.service';
import { ProjectsService } from '../../Services/projects.service';

interface TetraAnimation {
  mesh: THREE.Mesh;
  initialPosition: THREE.Vector3;
  rotationSpeed: THREE.Vector3;
  floatSpeed: number;
  floatAmplitude: number;
  phase: number;
}

interface IconInfo {
  id: string,
  icon: string,
  title: string,
  infos: string,
  show: boolean,
  x?: number,
  y?: number,
  expandTo?: 'left-up' | 'left-down' | 'right-up' | 'right-down' | 'center-up' | 'center-down',
} 

interface Project {
  id: string,
  name: string,
  description: string,
  url: string,
  type: number,
  start: string,
  end: string,
  iconsTech: string[]
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit, AfterViewInit {

  tech = inject(TechnologiesService);
  tecnologies: IconInfo[] = [];

  proj = inject(ProjectsService);
  projects: Project[] = [];

  ngOnInit(): void {
    this.tech.getTechnologies().subscribe((data: any) => {
      this.tecnologies = data;
    });

    this.proj.getProjects().subscribe((data: any) => {
      this.projects = data;
    });
  }

 headerHidden:boolean = false
 lastPositionMouse = 0;

 @ViewChild('canvasContainer', {static: true})
 private canvasContainer!: ElementRef<HTMLDivElement>;

 texto = 'Bokargiu';

 private scene!: THREE.Scene;
 private cam!: THREE.Camera;
 private render!: THREE.WebGLRenderer;

  private tetrahedrons: THREE.Mesh[] = [];
  private tetraAnimations: TetraAnimation[] = [];
  private connectionLines!: THREE.LineSegments;
  private connectionRadius = 2;

  private fps = 25;
  private frameInterval = 1000 / this.fps;
  private lastFrameTime = 0;

 ngAfterViewInit(): void {
     this.initThree();
 }

  @HostListener('window:scroll')
  onScroll(): void {
    const currentScrollPosition = window.scrollY;

    if (currentScrollPosition > this.lastPositionMouse) {
      this.headerHidden = true;
    } 
    else {
      this.headerHidden = false;
    }
    this.lastPositionMouse = currentScrollPosition;
  }

  private initThree(){
    const container = this.canvasContainer.nativeElement;

    this.scene = new THREE.Scene();
    this.cam = new THREE.PerspectiveCamera(
      80,
      container.clientWidth / container.clientHeight,
      0.1,
      0
    )
    this.cam.position.setZ(5);

    this.render = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true
    })

    this.render.setSize(
      container.clientWidth -1,
      container.clientHeight -1
    );

    this.render.setPixelRatio(
      Math.min(window.devicePixelRatio, 2)
    );

    container.appendChild(this.render.domElement);
    
    this.scene.add(this.addTetraton_v2())

    this.connectionLines = this.createConnections_v2();

    this.scene.add(this.connectionLines);

    this.animate(this.frameInterval);
  }

  private addTetraton_v2 = () => {

  const group = new THREE.Group();

  const geometry = new THREE.TetrahedronGeometry(0.3, 1);

  const material = new THREE.MeshBasicMaterial({
    color: 0x025623
  });

  for (let i = 0; i < 1000; i++) {

    const mesh = new THREE.Mesh(
      geometry,
      material
    );

    mesh.position.set(
      (Math.random() - 0.5) * 21,
      (Math.random() - 0.5) * 21,
      (Math.random() - 0.5) * 21
    );

    mesh.rotation.set(
      Math.random() * Math.PI,
      Math.random() * Math.PI,
      Math.random() * Math.PI
    );

    mesh.scale.setScalar(0.15);

    this.tetraAnimations.push({
      mesh,

      initialPosition: mesh.position.clone(),

      rotationSpeed: new THREE.Vector3(
        (Math.random() - 0.5) * 0.02,
        (Math.random() - 0.5) * 0.02,
        (Math.random() - 0.5) * 0.02
      ),

      floatSpeed:
        0.5 + Math.random() * 0.5,

      floatAmplitude:
        0.1 + Math.random() * 0.3,

      phase:
        Math.random() * Math.PI * 0.5
    });

    group.add(mesh);
  }

  return group;
}

private createConnections_v2(): THREE.LineSegments {

  const positions: number[] = [];

  for (let i = 0; i < this.tetraAnimations.length; i++) {

    const a = this.tetraAnimations[i].mesh;

    for (let j = i + 1; j < this.tetraAnimations.length; j++) {

      const b = this.tetraAnimations[j].mesh;

      const distance = a.position.distanceTo(b.position);

      if (distance <= this.connectionRadius) {

        positions.push(
          a.position.x,
          a.position.y,
          a.position.z,

          b.position.x,
          b.position.y,
          b.position.z
        );
      }
    }
  }

  const geometry = new THREE.BufferGeometry();

  geometry.setAttribute(
    'position',
    new THREE.Float32BufferAttribute(
      positions,
      3
    )
  );

  const material = new THREE.LineBasicMaterial({
    color: 0x16773c,
    transparent: true,
    opacity: 0.25
  });

  return new THREE.LineSegments(
    geometry,
    material
  );
}

private updateTetrahedrons(time: number) {

  for (const tetra of this.tetraAnimations) {

    const {
      mesh,
      initialPosition,
      rotationSpeed,
      floatSpeed,
      floatAmplitude,
      phase
    } = tetra;

    // Rotação
    mesh.rotation.x += rotationSpeed.x;
    mesh.rotation.y += rotationSpeed.y;
    mesh.rotation.z += rotationSpeed.z;

    // Movimento vertical
    mesh.position.y = initialPosition.y + Math.sin(
                      time * floatSpeed + phase
                    ) * floatAmplitude;

    // Movimento lateral
    mesh.position.x = initialPosition.x + Math.cos(
                      time * floatSpeed * 0.7 + phase
                    ) * floatAmplitude * 0.4;
  }
}

private updateConnections(): void {

  const positions: number[] = [];

  for (let i = 0; i < this.tetraAnimations.length; i++) {

    const a = this.tetraAnimations[i].mesh;

    for (let j = i + 1; j < this.tetraAnimations.length; j++) {

      const b = this.tetraAnimations[j].mesh;

      const distance = a.position.distanceTo(b.position);

      if (distance <= this.connectionRadius) {

        positions.push(
          a.position.x,
          a.position.y,
          a.position.z,

          b.position.x,
          b.position.y,
          b.position.z
        );
      }
    }
  }

  const geometry = this.connectionLines.geometry;

  geometry.setAttribute(
    'position',
    new THREE.Float32BufferAttribute(
      positions,
      3
    )
  );

  geometry.attributes['position'].needsUpdate = true;
}

  private animate = (currentTime: number) => {
    requestAnimationFrame(this.animate);

    const deltaTime = currentTime - this.lastFrameTime;

    if (deltaTime < this.frameInterval) {
      return;
    }

    this.lastFrameTime = currentTime - (deltaTime % this.frameInterval);

    const time = currentTime * 0.001;
    this.updateTetrahedrons(time);

    this.updateConnections();

    this.render.render(
      this.scene,
      this.cam
    )
  }

  expandeTech(event:MouseEvent, technologie: IconInfo, index: number){
    
    const icon = event.currentTarget as HTMLElement;
    const container = icon.parentElement as HTMLElement;

    const containerRect = container.getBoundingClientRect();

    let up = false;
    if(icon.getBoundingClientRect().y > window.innerHeight/2)
      up = true;

    switch(index%5){
      case 0:
      case 1:
        technologie.expandTo = up ? 'right-up' : 'right-down';
        technologie.x = containerRect.left;
        break;

      case 2:
        technologie.expandTo = up ? 'center-up' : 'center-down';
        technologie.x = containerRect.left + containerRect.width/2;
        break;

      default:
        technologie.expandTo = up ? 'left-up' : 'left-down';
        technologie.x = containerRect.right
        break;
    }
    technologie.y = up ? containerRect.top + window.scrollY : containerRect.top + window.scrollY + containerRect.height;
    technologie.show = true;
  }
}
