import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable, signal } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({ providedIn: 'root' })
export class BookingService {

    selectedServices = signal<any[]>([]);
    apiUrl = 'https://localhost:7189/api/Bookings';

    constructor(private http: HttpClient, private router: Router) { }

    formatDate(dateStr: Date): string {

        const date = new Date(dateStr);

        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const year = date.getFullYear();

        return `${month}/${day}/${year}`;
    }

    getMultiServiceStaffs(serviceIds: number[], date: Date)
    {
        let params = new HttpParams();

        serviceIds.forEach(id => {
            params = params.append('serviceIds', id);
        });

         params = params.append('date', this.formatDate(date));

          return this.http.get(`${this.apiUrl}/multi-service-auto-assign`, { params });

    }

    getMultiServiceSlots(serviceIds: number[], staffId: number | null, date: Date) {

        let params = new HttpParams();

        serviceIds.forEach(id => {
            params = params.append('serviceIds', id);
        });

        if (staffId) {
            params = params.append('staffId', staffId);
        }

        params = params.append('date', this.formatDate(date));

        return this.http.get(`${this.apiUrl}/multi-service-slots`, { params });
    }

    createBooking(request: any) {
        return this.http.post(`${this.apiUrl}/multi`, request);
    }
}