export interface CreateStaffRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone: string;
  bio: string;
  serviceIds: number[];
}