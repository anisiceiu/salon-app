using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IWorkingHoursRepository
{
    Task<IEnumerable<WorkingHours>> GetByStaffIdAsync(int staffId);
    Task<WorkingHours?> GetByStaffIdAndDayOfWeekAsync(int staffId, int dayOfWeek);
    Task<WorkingHours?> GetByIdAsync(int id);
    Task<WorkingHours> AddAsync(WorkingHours workingHours);
    Task<WorkingHours> UpdateAsync(WorkingHours workingHours);
    Task DeleteAsync(int id);
    Task DeleteByStaffIdAsync(int staffId);
}
