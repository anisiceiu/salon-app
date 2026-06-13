import { Component, inject, signal } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ServicesService } from '../../core/services/services.service';
import { CommonModule } from '@angular/common';
import { StaffService } from '../../core/services/staff.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateStaffRequest } from '../../core/models/CreateStaffRequest';

@Component({
  selector: 'app-add-staff',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './add-staff.html',
  styleUrl: './add-staff.css',
})
export class AddStaff {
   services = signal(Array<any>());
   toastr = inject(ToastrService);
   servicesService = inject(ServicesService);
   staffService = inject(StaffService);
   staff=signal<any>(null);
   staffForm!: FormGroup;

   constructor() {

    this.staffForm = new FormGroup({
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      phone: new FormControl(''),
      isActive: new FormControl(true),
      bio: new FormControl(''),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      serviceIds: new FormControl([])
    });

    this.servicesService.getAllServices().subscribe(data => {
        this.services.set(data as any);
        console.log(data);
      });
   }

   saveStaff() {
    const request = this.staffForm.value;
    let data:CreateStaffRequest = {
      firstName: request.firstName,
      lastName: request.lastName,
      email: request.email,
      phone: request.phone,
      isActive: request.isActive,
      bio: request.bio,
      password: request.password,
      serviceIds: request.serviceIds
    };
    this.staffService.createStaff(data).subscribe(() => {
      this.toastr.success('Staff added successfully');
    });
   }

   onServiceChange(event: Event, serviceId: number) {
    const checkbox = event.target as HTMLInputElement;
    const serviceIds = this.staffForm.get('serviceIds')?.value as number[];

    if (checkbox.checked) {
      this.staffForm.patchValue({
        serviceIds: [...serviceIds, serviceId]
      });
      return;
    }

    this.staffForm.patchValue({
      serviceIds: serviceIds.filter(id => id !== serviceId)
    });
   }
}
