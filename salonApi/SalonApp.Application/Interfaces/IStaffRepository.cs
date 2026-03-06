using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IStaffRepository
{
    Task<IEnumerable<Staff>> GetAllAsync();
    Task<Staff?> GetByIdAsync(int id);
    Task<Staff?> GetByUserIdAsync(int userId);
    Task<Staff> AddAsync(Staff staff);
    Task<Staff> AddWithUserAsync(User user, Staff staff, IEnumerable<int>? serviceIds = null);
    Task<Staff> UpdateAsync(Staff staff);
    Task DeleteAsync(int id);
    Task DeleteWithUserAsync(int id);
}
