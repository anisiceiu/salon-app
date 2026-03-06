using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IStaffServiceRepository
{
    Task<IEnumerable<StaffService>> GetByStaffIdAsync(int staffId);
    Task<IEnumerable<StaffService>> GetByServiceIdAsync(int serviceId);
    Task AddRangeAsync(IEnumerable<StaffService> staffServices);
    Task DeleteByStaffIdAsync(int staffId);
}
