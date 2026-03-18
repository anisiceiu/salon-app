import { Component, inject, signal } from '@angular/core';
import { StaffService } from '../../core/services/staff.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-staff-details',
  imports: [CommonModule,FormsModule],
  templateUrl: './staff-details.html',
  styleUrl: './staff-details.css',
})
export class StaffDetails {
   staffService = inject(StaffService);
   staff=signal<null | any>(null);
   staffId: number | null = null;

   constructor(private route: ActivatedRoute) {
    
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.staffId = Number(params.get('id'));
      console.log('Route id (observable):', this.staffId);
      if(this.staffId)
      this.getStaff(this.staffId);
    });

    
  }

  getStaff(id:number) {
    this.staffService.getStaffById(id).subscribe((data:any) => {
      this.staff.set(data);
    });
  }
}
