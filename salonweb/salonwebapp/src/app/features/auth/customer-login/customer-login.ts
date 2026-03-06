import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-customer-login',
  imports: [RouterModule,ReactiveFormsModule],
  templateUrl: './customer-login.html',
  styleUrl: './customer-login.css',
})
export class CustomerLogin {
loginForm: FormGroup;
constructor(private fb: FormBuilder, private authService: AuthService) {

    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(1)]]
    });

  }

  login() {

    if (this.loginForm.invalid) {
      return;
    }

    const data = this.loginForm.value;

    this.authService.login(data);

  }

}
