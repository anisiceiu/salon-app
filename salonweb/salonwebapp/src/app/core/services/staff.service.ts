import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({ providedIn: 'root' })
export class StaffService {

  apiUrl = 'https://localhost:7189/api/Staff';

  constructor(private http: HttpClient, private router: Router) {}

 getCategories()
 {
    return this.http.get(`${this.apiUrl}/categories`);
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

 saveWorkingHours(staffId: number,data:any) {
  const payload = {
    workingHours: data.map((w:any) => ({
      dayOfWeek: w.dayOfWeek,
      startTime: w.startTime,   // must be "HH:mm"
      endTime: w.endTime,       // must be "HH:mm"
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