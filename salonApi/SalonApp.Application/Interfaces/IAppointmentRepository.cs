using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Appointment>> GetByStaffIdAsync(int staffId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Appointment>> GetByStaffIdAndDateAsync(int staffId, DateTime date);
    Task<Appointment> AddAsync(Appointment appointment);
    Task<Appointment> UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);
}
