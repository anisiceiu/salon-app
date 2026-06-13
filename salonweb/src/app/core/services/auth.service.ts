import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import {UserRole} from '../enums/UserRole';

@Injectable({ providedIn: 'root' })
export class AuthService {

  apiUrl = 'https://localhost:7189/api/auth';

  constructor(private http: HttpClient, private router: Router) {}

  register(data:any)
  {
     this.http.post(`${this.apiUrl}/register`, data)
      .subscribe({
        next: (res:any) => {

          console.log('Register success', res);

          alert('Registration successful');

          this.router.navigate(['/customer-login']);

        },
        error: (err:any) => {
          console.log(err);
          alert('Registration failed');
        }
      });

  }

  login(data: any) {

    this.http.post<any>(`${this.apiUrl}/login`, data)
      .subscribe({
        next: (res:any) => {

          // Save token
          localStorage.setItem('token', res.token);
          localStorage.setItem('refreshToken', res.refreshToken);
          // Save role
          let userRole=UserRole[res.role];
          localStorage.setItem('role', userRole);

          // Redirect based on role
          if (userRole === 'Admin') {
            this.router.navigate(['/admin-dashboard']);
          }
          else if (userRole === 'Customer') {
            this.router.navigate(['/customer-dashboard']);
          }
          else if (userRole === 'Staff') {
            this.router.navigate(['/staff-dashboard']);
          }

        },
        error: (err:any) => {
          console.log('Login failed', err);
        }
      });

  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  refreshToken(){

    const refreshToken = localStorage.getItem('refreshToken');

    return this.http.post<any>('api/auth/refresh-token',{
      refreshToken: refreshToken
    });

  }

  logout() {
    localStorage.removeItem('token');
  }
}