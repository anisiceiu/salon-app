import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StaffService } from '../../../core/services/staff.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-working-hours',
  imports: [FormsModule],
  templateUrl: './working-hours.html',
  styleUrl: './working-hours.css',
})
export class WorkingHours {
  staffService = inject(StaffService);
  toastr = inject(ToastrService);
  staffs = signal(Array<any>());
  
  staffId:number=0;
  daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  workingHours = [
    { dayOfWeek: 0, startTime: '09:00', endTime: '17:00', isWorking: true },
    { dayOfWeek: 1, startTime: '09:00', endTime: '17:00', isWorking: true },
    { dayOfWeek: 2, startTime: '09:00', endTime: '17:00', isWorking: true },
    { dayOfWeek: 3, startTime: '09:00', endTime: '17:00', isWorking: true },
    { dayOfWeek: 4, startTime: '09:00', endTime: '17:00', isWorking: true },
    { dayOfWeek: 5, startTime: '10:00', endTime: '15:00', isWorking: true },
    { dayOfWeek: 6, startTime: '00:00', endTime: '00:00', isWorking: false }
  ];

  ngOnInit()
  {
      this.getStaffs();
  }


  getStaffs() {
    this.staffService.getStaffs().subscribe((data: any) => {
      this.staffs.set(data);
      console.log(data);
    });
  }

  save() {
    this.staffService.saveWorkingHours(this.staffId, this.workingHours).subscribe((data: any) => {
      
        this.toastr.success('Saved successfully');
      
    });
  }

}
