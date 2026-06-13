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
  selectedStaff = signal<any | null>(null);
  selectedDate = signal('');
  selectedTime = signal('');
  staffService = inject(StaffService);
  toastr = inject(ToastrService);
  staffs = signal(Array<any>());
  weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  constructor(public booking: BookingService) {
    console.log('selected:', booking.selectedServices());
    if (booking.selectedServices()) {
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
    if(step == 2)
    {
      this.getStaffs();
    }
  }

  toggleService(service: any) {
    this.selectedServices.update(list => {
      const exists = list.find(s => s.id === service.id);

      if (exists) {
        return list.filter(s => s.id !== service.id);
      }

      return [...list, service];
    });

    this.booking.selectedServices.set(this.selectedServices());
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
    console.log('selected:', this.booking.selectedServices());
    let serviceIds = this.selectedServices().map(s => s.id);
    this.getAvailableSlots(serviceIds, this.selectedStaff(), new Date(this.selectedDate()));
  }

  // ----- Time slots -----

  timeSlots = signal<any[]>([
    {
      startTime: '2026-03-11T09:00:00',
      endTime: '2026-03-11T11:50:00',
      totalDurationMinutes: 170,
      totalPrice: 120,
      isAvailable: false
    },
    {
      startTime: '2026-03-11T09:30:00',
      endTime: '2026-03-11T12:20:00',
      totalDurationMinutes: 170,
      totalPrice: 120,
      isAvailable: false
    },
    {
      startTime: '2026-03-11T10:00:00',
      endTime: '2026-03-11T12:50:00',
      totalDurationMinutes: 170,
      totalPrice: 120,
      isAvailable: false
    },
    {
      startTime: '2026-03-11T10:30:00',
      endTime: '2026-03-11T13:20:00',
      totalDurationMinutes: 170,
      totalPrice: 120,
      isAvailable: false
    },
    {
      startTime: '2026-03-11T11:00:00',
      endTime: '2026-03-11T13:50:00',
      totalDurationMinutes: 170,
      totalPrice: 120,
      isAvailable: false
    }
  ]);

  selectTime(time: string) {
    this.selectedTime.set(time);
  }



  selectedSlot = signal<any | null>(null);

  selectSlot(slot: any) {
    this.selectedSlot.set(slot);
  }
  getAvailableSlots(serviceIds: number[], staffId: number, date: Date) {
    this.booking.getMultiServiceSlots(serviceIds, staffId, date).subscribe((data: any) => {
      this.timeSlots.set(data);
    });
  }

  getStaffs() {
    let serviceIds = this.selectedServices().map(s => s.id);
    this.booking.getMultiServiceStaffs(serviceIds,new Date(12,2,2026)).subscribe((data: any) => {
      this.staffs.set(data);
    });
  }

  selectedStaffObj = computed(() => {
    return this.staffs().find(c => c.id == this.selectedStaff());
  });


  confirmBooking() {
    let serviceIds = this.selectedServices().map(s => s.id);
    const bookingRequest = {
      serviceIds: serviceIds,
      preferredStaffId: this.selectedStaff() == 0 ? null : this.selectedStaff(),
      dateTime: this.selectedSlot()?.startTime ?? this.selectedDate(),
      notes: 'Customer prefers senior barber'
    };

    this.booking.createBooking(bookingRequest)
      .subscribe({
        next: res => {
          this.toastr.success("Booking Saved sucessfully");
        },
        error: err => {
          this.toastr.error(err?.error?.message);

        }
      });
  }

}
