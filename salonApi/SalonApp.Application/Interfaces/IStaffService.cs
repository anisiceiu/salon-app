using SalonApp.Application.DTOs;

namespace SalonApp.Application.Interfaces;

public interface IStaffService
{
    // Staff CRUD
    Task<IEnumerable<StaffDto>> GetAllStaffAsync();
    Task<StaffDto?> GetStaffByIdAsync(int id);
    Task<StaffDto> CreateStaffAsync(CreateStaffRequest request);
    Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffRequest request);
    Task<bool> DeleteStaffAsync(int id);
    
    // Services assignment
    Task<bool> AssignServicesAsync(int staffId, AssignServicesRequest request);
    Task<IEnumerable<ServiceDto>> GetStaffServicesAsync(int staffId);
    
    // Working hours
    Task<IEnumerable<WorkingHoursDto>> GetWorkingHoursAsync(int staffId);
    Task<bool> SetWorkingHoursAsync(int staffId, SetWorkingHoursRequest request);
    
    // Leave management
    Task<LeaveDto> RequestLeaveAsync(int staffId, CreateLeaveRequest request);
    Task<LeaveDto?> ApproveLeaveAsync(int leaveId, ApproveLeaveRequest request);
    Task<IEnumerable<LeaveDto>> GetAllLeavesAsync();
    Task<IEnumerable<LeaveDto>> GetMyLeavesAsync(int staffId);
}
