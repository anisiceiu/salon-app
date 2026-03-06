import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-customer-register',
  imports: [RouterModule,ReactiveFormsModule],
  templateUrl: './customer-register.html',
  styleUrl: './customer-register.css',
})
export class CustomerRegister {
registerForm: FormGroup;
authService=inject(AuthService);

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {

    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(1)]],
      fullName: ['', Validators.required],
      phone: ['', Validators.required],
      confirmPassword:['',Validators.required]
    });

  }

  register() {

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    if(this.registerForm.controls['password'].value != this.registerForm.controls['confirmPassword'].value)
    {
      return;
    }

    const data = this.registerForm.value;
    this.authService.register(data);
   
  }

}
