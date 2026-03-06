using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Domain.Enums;

namespace SalonApp.Application.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IWorkingHoursRepository _workingHoursRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly IStaffServiceRepository _staffServiceRepository;
    private readonly IUserRepository _userRepository;

    public StaffService(
        IStaffRepository staffRepository,
        IWorkingHoursRepository workingHoursRepository,
        ILeaveRepository leaveRepository,
        IStaffServiceRepository staffServiceRepository,
        IUserRepository userRepository)
    {
        _staffRepository = staffRepository;
        _workingHoursRepository = workingHoursRepository;
        _leaveRepository = leaveRepository;
        _staffServiceRepository = staffServiceRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
    {
        var staff = await _staffRepository.GetAllAsync();
        return staff.Select(MapToStaffDto).ToList();
    }

    public async Task<StaffDto?> GetStaffByIdAsync(int id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);
        return staff == null ? null : MapToStaffDto(staff);
    }

    public async Task<StaffDto> CreateStaffAsync(CreateStaffRequest request)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Create User
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = $"{request.FirstName} {request.LastName}",
            Phone = request.Phone,
            Role = UserRole.Staff,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Create Staff
        var staff = new Staff
        {
            Bio = request.Bio,
            IsAvailable = true
        };

        // Save staff with user
        var createdStaff = await _staffRepository.AddWithUserAsync(user, staff, request.ServiceIds);
        
        return MapToStaffDto(createdStaff);
    }

    public async Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffRequest request)
    {
        var staff = await _staffRepository.GetByIdAsync(id);
        if (staff == null)
        {
            throw new InvalidOperationException("Staff not found");
        }

        var user = staff.User;
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Update user info
        user.FullName = $"{request.FirstName} {request.LastName}";
        user.Phone = request.Phone;
        user.IsActive = request.IsActive;
        
        await _userRepository.UpdateAsync(user);

        // Update staff info
        staff.Bio = request.Bio;
        staff.ProfileImage = request.PhotoUrl;
        staff.IsAvailable = request.IsActive;

        await _staffRepository.UpdateAsync(staff);

        // Update services if provided
        if (request.ServiceIds != null)
        {
            await _staffServiceRepository.DeleteByStaffIdAsync(id);
            
            if (request.ServiceIds.Any())
            {
                var staffServices = request.ServiceIds.Select(serviceId => new Domain.Entities.StaffService
                {
                    StaffId = id,
                    ServiceId = serviceId
                });

                await _staffServiceRepository.AddRangeAsync(staffServices);
            }
        }

        // Reload staff with includes
        var updatedStaff = await _staffRepository.GetByIdAsync(id);
        return MapToStaffDto(updatedStaff!);
    }

    public async Task<bool> DeleteStaffAsync(int id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);
        if (staff == null)
        {
            return false;
        }

        await _staffRepository.DeleteWithUserAsync(id);
        return true;
    }

    public async Task<bool> AssignServicesAsync(int staffId, AssignServicesRequest request)
    {
        var staff = await _staffRepository.GetByIdAsync(staffId);
        if (staff == null)
        {
            throw new InvalidOperationException("Staff not found");
        }

        // Delete existing services
        await _staffServiceRepository.DeleteByStaffIdAsync(staffId);

        // Add new services
        if (request.ServiceIds.Any())
        {
            var staffServices = request.ServiceIds.Select(serviceId => new Domain.Entities.StaffService
            {
                StaffId = staffId,
                ServiceId = serviceId
            });

            await _staffServiceRepository.AddRangeAsync(staffServices);
        }

        return true;
    }

    public async Task<IEnumerable<ServiceDto>> GetStaffServicesAsync(int staffId)
    {
        var staffServices = await _staffServiceRepository.GetByStaffIdAsync(staffId);
        
        return staffServices.Select(ss => new ServiceDto
        {
            Id = ss.Service!.Id,
            Name = ss.Service.Name,
            Description = ss.Service.Description,
            Duration = ss.Service.Duration,
            Price = ss.Service.Price,
            CategoryId = ss.Service.CategoryId,
            CategoryName = ss.Service.Category?.Name,
            IsActive = ss.Service.IsActive,
            CreatedAt = ss.Service.CreatedAt
        }).ToList();
    }

    public async Task<IEnumerable<WorkingHoursDto>> GetWorkingHoursAsync(int staffId)
    {
        var workingHours = await _workingHoursRepository.GetByStaffIdAsync(staffId);
        return workingHours.Select(MapToWorkingHoursDto).ToList();
    }

    public async Task<bool> SetWorkingHoursAsync(int staffId, SetWorkingHoursRequest request)
    {
        var staff = await _staffRepository.GetByIdAsync(staffId);
        if (staff == null)
        {
            throw new InvalidOperationException("Staff not found");
        }

        // Validate working hours
        foreach (var wh in request.WorkingHours)
        {
            if (wh.IsWorking && wh.EndTime <= wh.StartTime)
            {
                throw new InvalidOperationException($"End time must be greater than start time for day {wh.DayOfWeek}");
            }
        }

        // Delete existing working hours
        await _workingHoursRepository.DeleteByStaffIdAsync(staffId);

        // Add new working hours
        foreach (var wh in request.WorkingHours)
        {
            var workingHour = new WorkingHours
            {
                StaffId = staffId,
                DayOfWeek = wh.DayOfWeek,
                StartTime = wh.StartTime,
                EndTime = wh.EndTime,
                IsWorking = wh.IsWorking
            };

            await _workingHoursRepository.AddAsync(workingHour);
        }

        return true;
    }

    public async Task<LeaveDto> RequestLeaveAsync(int staffId, CreateLeaveRequest request)
    {
        var staff = await _staffRepository.GetByIdAsync(staffId);
        if (staff == null)
        {
            throw new InvalidOperationException("Staff not found");
        }

        if (request.EndDate < request.StartDate)
        {
            throw new InvalidOperationException("End date must be greater than or equal to start date");
        }

        var leave = new Leave
        {
            StaffId = staffId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _leaveRepository.AddAsync(leave);

        return await GetLeaveDtoAsync(leave);
    }

    public async Task<LeaveDto?> ApproveLeaveAsync(int leaveId, ApproveLeaveRequest request)
    {
        var leave = await _leaveRepository.GetByIdAsync(leaveId);
        if (leave == null)
        {
            return null;
        }

        leave.Status = request.Approved ? LeaveStatus.Approved : LeaveStatus.Rejected;
        
        await _leaveRepository.UpdateAsync(leave);

        return await GetLeaveDtoAsync(leave);
    }

    public async Task<IEnumerable<LeaveDto>> GetAllLeavesAsync()
    {
        var leaves = await _leaveRepository.GetAllAsync();
        var leaveDtos = new List<LeaveDto>();
        
        foreach (var leave in leaves)
        {
            leaveDtos.Add(await GetLeaveDtoAsync(leave));
        }
        
        return leaveDtos;
    }

    public async Task<IEnumerable<LeaveDto>> GetMyLeavesAsync(int staffId)
    {
        var leaves = await _leaveRepository.GetByStaffIdAsync(staffId);
        var leaveDtos = new List<LeaveDto>();
        
        foreach (var leave in leaves)
        {
            leaveDtos.Add(await GetLeaveDtoAsync(leave));
        }
        
        return leaveDtos;
    }

    private async Task<LeaveDto> GetLeaveDtoAsync(Leave leave)
    {
        var staff = await _staffRepository.GetByIdAsync(leave.StaffId);
        
        return new LeaveDto
        {
            Id = leave.Id,
            StaffId = leave.StaffId,
            StaffName = staff?.User?.FullName,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            Status = leave.Status,
            CreatedAt = leave.CreatedAt
        };
    }

    private StaffDto MapToStaffDto(Staff staff)
    {
        var dayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        
        return new StaffDto
        {
            Id = staff.Id,
            UserId = staff.Id,
            FirstName = staff.User?.FullName?.Split(' ').FirstOrDefault() ?? "",
            LastName = staff.User?.FullName?.Split(' ').Skip(1).FirstOrDefault() ?? "",
            Email = staff.User?.Email ?? "",
            Phone = staff.User?.Phone ?? "",
            Bio = staff.Bio,
            PhotoUrl = staff.ProfileImage,
            IsActive = staff.User?.IsActive ?? false,
            Services = staff.StaffServices?.Select(ss => new ServiceDto
            {
                Id = ss.Service!.Id,
                Name = ss.Service.Name,
                Description = ss.Service.Description,
                Duration = ss.Service.Duration,
                Price = ss.Service.Price,
                CategoryId = ss.Service.CategoryId,
                CategoryName = ss.Service.Category?.Name,
                IsActive = ss.Service.IsActive,
                CreatedAt = ss.Service.CreatedAt
            }).ToList() ?? new List<ServiceDto>(),
            WorkingHours = staff.WorkingHours?.Select(wh => new WorkingHoursDto
            {
                Id = wh.Id,
                DayOfWeek = wh.DayOfWeek,
                StartTime = wh.StartTime,
                EndTime = wh.EndTime,
                IsWorking = wh.IsWorking,
                DayName = dayNames[wh.DayOfWeek]
            }).ToList() ?? new List<WorkingHoursDto>()
        };
    }

    private WorkingHoursDto MapToWorkingHoursDto(WorkingHours wh)
    {
        var dayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        
        return new WorkingHoursDto
        {
            Id = wh.Id,
            DayOfWeek = wh.DayOfWeek,
            StartTime = wh.StartTime,
            EndTime = wh.EndTime,
            IsWorking = wh.IsWorking,
            DayName = dayNames[wh.DayOfWeek]
        };
    }
}
