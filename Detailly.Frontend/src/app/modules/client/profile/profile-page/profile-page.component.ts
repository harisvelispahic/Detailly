import { Component } from '@angular/core';

@Component({
  selector: 'app-profile-page',
  standalone: false,
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss',
})
export class ProfilePageComponent {
  readonly vehicles = [
    { name: '2022 BMW M3', plate: 'ABC-1234' },
    { name: '2025 Audi R8', plate: 'XYZ-5678' },
  ];
}
