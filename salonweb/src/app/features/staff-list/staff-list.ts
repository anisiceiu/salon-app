import { Component, inject, signal } from '@angular/core';
import { StaffService } from '../../core/services/staff.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-staff-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './staff-list.html',
  styleUrl: './staff-list.css',
})
export class StaffList {
  staffService = inject(StaffService);
  staffs=signal(Array<any>());

  constructor() {
    this.getStaffs();
  }

  getStaffs() {
    this.staffService.getStaffs().subscribe((data:any) => {
      this.staffs.set(data);
    });
  }

  deleteStaff(id:number)
  {

  }

  viewSchedule(staff:any)
  {
    
  }
}
