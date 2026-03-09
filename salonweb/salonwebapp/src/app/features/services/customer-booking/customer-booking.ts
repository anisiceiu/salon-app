import { Component, computed, inject, signal } from '@angular/core';
import { ServicesService } from '../../../core/services/services.service';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../../core/services/booking.service';
import { StaffService } from '../../../core/services/staff.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer-booking',
  imports: [CommonModule],
  templateUrl: './customer-booking.html',
  styleUrl: './customer-booking.css',
})
export class CustomerBooking {
  currentStep = signal(1);
  servicesService = inject(ServicesService);
  services = signal(Array<any>());
  selectedServices = signal<any[]>([]);
  selectedStaff = signal(0);
  selectedDate = signal('');
  selectedTime = signal('');
  staffService = inject(StaffService);
  toastr = inject(ToastrService);
  staffs = signal(Array<any>());
  weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  constructor(public booking: BookingService)
  {
    console.log('selected:',booking.selectedServices);
    if(booking.selectedServices())
    {
      this.selectedServices.set(this.booking.selectedServices());
    }
    this.getServices();
    this.getStaffs();
  }

  getServices() {

    this.servicesService.getAllServices().subscribe(data => {
      this.services.set(data as any);
      console.log(data);
    });
  }

  

  cartTotal = computed(() =>
    this.selectedServices().reduce((sum, s) => sum + s.price, 0)
  );

  goToStep(step: number) {
    this.currentStep.set(step);
  }

  toggleService(service: any) {
    this.selectedServices.update(list => {
      const exists = list.find(s => s.id === service.id);

      if (exists) {
        return list.filter(s => s.id !== service.id);
      }

      return [...list, service];
    });
  }

  selectStaff(staffId: number) {
    this.selectedStaff.set(staffId);
  }


  today = new Date();

  currentMonth = signal(this.today.getMonth());
  currentYear = signal(this.today.getFullYear());

  bookedDates = [''
  ];

  days = computed(() => {

    const month = this.currentMonth();
    const year = this.currentYear();

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    const daysInMonth = lastDay.getDate();
    const startDay = firstDay.getDay();

    const result: any[] = [];

    for (let i = 0; i < startDay; i++) {
      result.push(null);
    }

    for (let d = 1; d <= daysInMonth; d++) {

      const dateStr =
        `${year}-${String(month + 1).padStart(2, '0')}-${String(d).padStart(2, '0')}`;

      const isPast =
        new Date(year, month, d) <
        new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());

      const isBooked = this.bookedDates.includes(dateStr);

      result.push({
        day: d,
        date: dateStr,
        disabled: isPast || isBooked
      });
    }

    return result;

  });

  changeMonth(delta: number) {

    let m = this.currentMonth() + delta;
    let y = this.currentYear();

    if (m > 11) {
      m = 0;
      y++;
    }

    if (m < 0) {
      m = 11;
      y--;
    }

    this.currentMonth.set(m);
    this.currentYear.set(y);
  }

  selectDate(date: string) {
    this.selectedDate.set(date);
    this.getAvailableSlots(2,1,new Date(this.selectedDate()));
  }

  // ----- Time slots -----

  timeSlots = signal([
    { time: '09:00', available: false },
    { time: '09:30', available: false },
    { time: '10:00', available: false },
    { time: '10:30', available: false },
    { time: '11:00', available: false },
    { time: '11:30', available: false },
    { time: '12:00', available: false },
    { time: '12:30', available: false },
    { time: '13:00', available: false },
    { time: '13:30', available: false },
    { time: '14:00', available: false },
    { time: '14:30', available: false },
    { time: '15:00', available: false },
    { time: '15:30', available: false },
    { time: '16:00', available: false },
    { time: '16:30', available: false },
    { time: '17:00', available: false },
    { time: '17:30', available: false },
    { time: '18:00', available: false }
  ]);

  selectTime(time: string) {
    this.selectedTime.set(time);
  }

  loadSlots(apiData: any[]) {

  const slots = apiData.map(x => ({
    time: new Date(x.startTime).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }),
    available: x.isAvailable
  }));

  this.timeSlots.set(slots);

}


getAvailableSlots(staffId:number,serviceId:number,date:Date)
{
  this.booking.getAvailableSlots(staffId,serviceId,date).subscribe((data:any)=>{
     this.loadSlots(data);
  });
}

getStaffs() {
    this.staffService.getStaffs().subscribe((data: any) => {
      this.staffs.set(data);
    });
  }

}
