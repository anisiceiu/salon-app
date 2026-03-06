import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({ providedIn: 'root' })
export class ServicesService {

  apiUrl = 'https://localhost:7189/api/Services';

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

 updateService(id:number,data:any)
 {
   return this.http.put(`${this.apiUrl}/${id}`,data);
 }

  deleteService(id:number)
 {
   return this.http.delete(`${this.apiUrl}/${id}`);
 }


}