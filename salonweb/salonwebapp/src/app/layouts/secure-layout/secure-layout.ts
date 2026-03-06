import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-secure-layout',
  imports: [CommonModule,RouterModule],
  templateUrl: './secure-layout.html',
  styleUrl: './secure-layout.css',
})
export class SecureLayout {
  authService = inject(AuthService);
  constructor(private router:Router)
  {

  }

  logout()
  {
    this.authService.logout();
    this.router.navigateByUrl("/admin-login");
  }

}
