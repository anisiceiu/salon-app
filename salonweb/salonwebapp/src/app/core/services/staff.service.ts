import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({ providedIn: 'root' })
export class StaffService {

  apiUrl = 'https://localhost:7189/api/Staff';

  constructor(private http: HttpClient, private router: Router) {}

 getStaffs()
 {
    return this.http.get(`${this.apiUrl}`);
 }

 getServicesByCategoryId(id:number | null)
 {
    return this.http.get(`${this.apiUrl}/category/${id}`);
 }

 getAllServices()
 {
    return this.http.get(`${this.apiUrl}`);
 }

 getServiceById(id:number)
 {
   return this.http.get(`${this.apiUrl}/${id}`);
 }

 addService(data:any)
 {
   return this.http.post(this.apiUrl,data);
 }

formatTime(time: string): string {
  if (!time) return '';
  return time.length === 5 ? time + ':00' : time;
}

 saveWorkingHours(staffId: number,data:any) {
  const payload = {
    workingHours: data.map((w:any) => ({
      dayOfWeek: w.dayOfWeek,
      startTime: this.formatTime(w.startTime),   // must be "HH:mm"
      endTime: this.formatTime(w.endTime),       // must be "HH:mm"
      isWorking: w.isWorking
    }))
  };

  return this.http.put(`${this.apiUrl}/${staffId}/working-hours`, payload, {
    headers: { 'Content-Type': 'application/json' }
  });
}

  deleteService(id:number)
 {
   return this.http.delete(`${this.apiUrl}/${id}`);
 }


}