import { Component, inject, signal } from '@angular/core';
import { ServicesService } from '../../../core/services/services.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from "@angular/router";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-services',
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-services.html',
  styleUrl: './admin-services.css',
})
export class AdminServices {
  servicesService = inject(ServicesService);
  categories = signal(Array<any>());
  services = signal(Array<any>());
  selectedCategory = signal<number | null>(null);
  toastr = inject(ToastrService);

  constructor() {

  }

  selectCategory(id: number | null) {
    this.selectedCategory.set(id);
    if (this.selectedCategory()) {
      this.servicesService.getServicesByCategoryId(this.selectedCategory()).subscribe(data => {
        this.services.set(data as any);
        console.log(data);
      });
    }
    else {
      this.servicesService.getAllServices().subscribe(data => {
        this.services.set(data as any);
        console.log(data);
      });
    }
  }

  ngOnInit() {
    this.servicesService.getCategories().subscribe(data => {
      this.categories.set(data as any);
    });

    this.selectCategory(null);

  }

  deleteService(id: number) {
    this.servicesService.deleteService(id).subscribe(data => {
      this.toastr.success("Deleted successfully!");
      this.selectCategory(this.selectedCategory());
    });
  }

}
