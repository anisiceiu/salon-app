using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface ILeaveRepository
{
    Task<IEnumerable<Leave>> GetAllAsync();
    Task<Leave?> GetByIdAsync(int id);
    Task<IEnumerable<Leave>> GetByStaffIdAsync(int staffId);
    Task<IEnumerable<Leave>> GetByStaffIdAndDateRangeAsync(int staffId, DateTime startDate, DateTime endDate);
    Task<Leave> AddAsync(Leave leave);
    Task<Leave> UpdateAsync(Leave leave);
    Task DeleteAsync(int id);
}
