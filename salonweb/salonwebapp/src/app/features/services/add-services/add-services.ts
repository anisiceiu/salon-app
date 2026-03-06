import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ServicesService } from '../../../core/services/services.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-services',
  imports: [ReactiveFormsModule],
  templateUrl: './add-services.html',
  styleUrl: './add-services.css',
})
export class AddServices {
  serviceId: number | null = null;
  service = signal<any>(null);
  serviceForm!: FormGroup;
  servicesService = inject(ServicesService);
  categories = signal(Array<any>());
  toastr = inject(ToastrService);



  constructor(private route: ActivatedRoute) {
    this.serviceForm = new FormGroup({
      name: new FormControl('', [Validators.required]),
      categoryId: new FormControl('', [Validators.required]),
      price: new FormControl(0, [Validators.required, Validators.min(0)]),
      duration: new FormControl(30, [Validators.required, Validators.min(1)]),
      description: new FormControl(''),
      imageUrl: new FormControl('', Validators.pattern('https?://.+')),
      isActive: new FormControl(true)
    });
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.serviceId = Number(params.get('id'));
      console.log('Route id (observable):', this.serviceId);
      if(this.serviceId)
      this.getServiceById(this.serviceId);
    });

    this.servicesService.getCategories().subscribe(data => {
      this.categories.set(data as any);
    });
  }

  getServiceById(id: number) {
    this.servicesService.getServiceById(id).subscribe((data: any) => {
      this.service.set(data as any);
      if (data) {
        this.serviceForm.patchValue({
          name: data.name,
          categoryId: data.categoryId,
          price: data.price,
          duration: data.duration,
          description: data.description,
          imageUrl: data.imageUrl,
          isActive: data.isActive

        });
      }
    });
  }

  onSubmit() {
    if (this.serviceForm.valid) {
      console.log('Service data:', this.serviceForm.value);
      // call API or save logic
      if (this.service() && this.service().id) {
        this.servicesService.updateService(this.service().id, this.serviceForm.value).subscribe(data=>{
         if(data)
         {
          this.toastr.success("Saved successfully!")
         }
         else{
          this.toastr.error("Cannot be saved!")
         }
        });
      }
      else {
        this.servicesService.addService(this.serviceForm.value).subscribe(data=>{
         if(data)
         {
          this.toastr.success("Saved successfully!")
         }
         else{
          this.toastr.error("Cannot be saved!")
         }
        });
      }


    } else {
      console.log('Form invalid!');
      this.serviceForm.markAllAsTouched();
      this.toastr.error("Form invalid!");
    }
  }

 

}
