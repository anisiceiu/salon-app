import { Injectable, signal } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class BookingService {

    selectedServices = signal<any[]>([]);

}