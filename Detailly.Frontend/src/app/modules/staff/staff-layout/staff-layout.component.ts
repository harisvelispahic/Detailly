import { Component, inject } from '@angular/core';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';

@Component({
  selector: 'app-staff-layout',
  standalone: false,
  templateUrl: './staff-layout.component.html',
  styleUrl: './staff-layout.component.scss',
})
export class StaffLayoutComponent {
  readonly auth = inject(AuthFacadeService);
}
