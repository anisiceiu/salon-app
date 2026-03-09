import { HttpClient } from "@angular/common/http";
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

    getAvailableSlots(staffId: number, serviceid: number, date: Date) {
        return this.http.get(`${this.apiUrl}/slots?staffId=${staffId}&serviceId=${serviceid}&date=${this.formatDate(date)}`);
    }
}