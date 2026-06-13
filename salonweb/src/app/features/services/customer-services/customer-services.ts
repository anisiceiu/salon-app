import { Component, computed, inject, signal } from '@angular/core';
import { ServicesService } from '../../../core/services/services.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { RouterLink } from "@angular/router";
import { BookingService } from '../../../core/services/booking.service';

@Component({
  selector: 'app-customer-services',
  imports: [CommonModule, RouterLink],
  templateUrl: './customer-services.html',
  styleUrl: './customer-services.css',
})
export class CustomerServices {
servicesService = inject(ServicesService);
  categories = signal(Array<any>());
  services = signal(Array<any>());
  selectedCategory = signal<number | null>(null);
  toastr = inject(ToastrService);
  groupedArray=signal(Array<any>());
  selectedServiceList=signal(Array<any>());
  
  constructor(public booking: BookingService) {

  }

  makeGroupedServices()
  {
    const grouped = this.services().reduce((acc, item) => {

  if (!acc[item.categoryName]) {
    acc[item.categoryName] = [];
  }

  acc[item.categoryName].push(item);

  return acc;

  }, {} as Record<string, any[]>);
  
  
  let ga = Object.entries(grouped).map(([categoryName, services]) => ({
  categoryName,
  services
  }));

  this.groupedArray.set(ga);
  }

  selectCategory(id: number | null) {

  this.selectedCategory.set(id);

  const request = id
    ? this.servicesService.getServicesByCategoryId(id)
    : this.servicesService.getAllServices();

  request.subscribe((data:any) => {
    this.services.set(data);
    this.makeGroupedServices();
  });


  }

  isSelected = (id: number) =>
  computed(() => this.selectedServiceList().some(s=> s.id==id));
  
  cartTotal = () =>
  computed(()=> this.selectedServiceList().reduce((a, b) => a + b.price, 0));

  toggleService(service:any)
  {
    if(this.selectedServiceList().every(s=> s.id !== service.id))
    {
      this.selectedServiceList.update(arr=>[...arr,service]);
    }
    else{
      this.selectedServiceList.update(arr=> arr.filter(s=> s.id!== service.id));
    }
  
    this.booking.selectedServices.set(this.selectedServiceList());
  }

  ngOnInit() {
    this.servicesService.getCategories().subscribe((data:any) => {
      this.categories.set(data as any);
    });

    this.selectCategory(null);

  }

  deleteService(id: number) {
    this.servicesService.deleteService(id).subscribe((data:any) => {
      this.toastr.success("Deleted successfully!");
      this.selectCategory(this.selectedCategory());
    });
  }

}
