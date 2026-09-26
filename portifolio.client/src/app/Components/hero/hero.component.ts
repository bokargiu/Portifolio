import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import * as THREE from 'three';

interface TetraAnimation {
  mesh: THREE.Mesh;
  initialPosition: THREE.Vector3;
  rotationSpeed: THREE.Vector3;
  floatSpeed: number;
  floatAmplitude: number;
  phase: number;
}

@Component({
  selector: 'app-hero',
  standalone: true,
  imports: [],
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.scss'
})
export class HeroComponent implements AfterViewInit {

  @ViewChild('canvasContainer', { static: true })
  private canvasContainer!: ElementRef<HTMLDivElement>;

  private scene!: THREE.Scene;
  private cam!: THREE.Camera;
  private render!: THREE.WebGLRenderer;

  private tetraAnimations: TetraAnimation[] = [];
  private connectionLines!: THREE.LineSegments;
  private connectionRadius = 2;

  private fps = 25;
  private frameInterval = 1000 / this.fps;
  private lastFrameTime = 0;

  ngAfterViewInit(): void {
    this.initThree();
  }

  private initThree() {
    const container = this.canvasContainer.nativeElement;

    this.scene = new THREE.Scene();
    this.cam = new THREE.PerspectiveCamera(
      80,
      container.clientWidth / container.clientHeight,
      0.1,
      0
    );
    this.cam.position.setZ(5);

    this.render = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true
    });

    this.render.setSize(
      container.clientWidth - 1,
      container.clientHeight - 1
    );

    this.render.setPixelRatio(
      Math.min(window.devicePixelRatio, 2)
    );

    container.appendChild(this.render.domElement);

    this.scene.add(this.addTetraton_v2());

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
        floatSpeed: 0.5 + Math.random() * 0.5,
        floatAmplitude: 0.1 + Math.random() * 0.3,
        phase: Math.random() * Math.PI * 0.5
      });

      group.add(mesh);
    }

    return group;
  };

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

      mesh.rotation.x += rotationSpeed.x;
      mesh.rotation.y += rotationSpeed.y;
      mesh.rotation.z += rotationSpeed.z;

      mesh.position.y = initialPosition.y + Math.sin(
        time * floatSpeed + phase
      ) * floatAmplitude;

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
    );
  };
}
