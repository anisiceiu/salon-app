import { Routes } from '@angular/router';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { PublicLayout } from './layouts/public-layout/public-layout';
import { SecureLayout } from './layouts/secure-layout/secure-layout';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    //auth layout
     { path: '', redirectTo: 'home', pathMatch: 'full' },
    {
    path: '',
    component: AuthLayout,
    children: [
      {
        path: 'admin-login',
        loadComponent: () =>
          import('./features/auth/admin-login/admin-login')
            .then(m => m.AdminLogin)
      },
       {
        path: 'customer-login',
        loadComponent: () =>
          import('./features/auth/customer-login/customer-login')
            .then(m => m.CustomerLogin)
      },
       {
        path: 'staff-login',
        loadComponent: () =>
          import('./features/auth/staff-login/staff-login')
            .then(m => m.StaffLogin)
      },
      {
        path: 'customer-register',
        loadComponent: () =>
          import('./features/auth/customer-register/customer-register')
            .then(m => m.CustomerRegister)
      },
    ]
  },
    // Public Layout
  {
    path: '',
    component: PublicLayout,
    children: [
      {
        path: 'home',
        loadComponent: () =>
          import('./features/home/home')
            .then(m => m.Home)
      },
     
    ]
  },

  // Secure Layout
  {
    path: '',
    component: SecureLayout,
    //canActivate: [authGuard],
    children: [
      {
        path: 'admin-dashboard',
        loadComponent: () =>
          import('./features/dashboard/admin/admin')
            .then(m => m.Admin)
      },
      {
        path: 'customer-dashboard',
        loadComponent: () =>
          import('./features/dashboard/customer/customer')
            .then(m => m.Customer)
      },
       {
        path: 'customer-services',
        loadComponent: () =>
          import('./features/services/customer-services/customer-services')
            .then(m => m.CustomerServices)
      },
      {
        path: 'customer-booking',
        loadComponent: () =>
          import('./features/services/customer-booking/customer-booking')
            .then(m => m.CustomerBooking)
      },
      {
        path: 'working-hours',
        loadComponent: () =>
          import('./features/services/working-hours/working-hours')
            .then(m => m.WorkingHours)
      },
      {
        path: 'admin-services',
        loadComponent: () =>
          import('./features/services/admin-services/admin-services')
            .then(m => m.AdminServices)
      },
      {
        path: 'add-service',
        loadComponent: () =>
          import('./features/services/add-services/add-services')
            .then(m => m.AddServices)
      },
      {
        path: 'add-service/:id',
        loadComponent: () =>
          import('./features/services/add-services/add-services')
            .then(m => m.AddServices)
      },
    ]
  },

{ path: '**', redirectTo: 'home' }
];
