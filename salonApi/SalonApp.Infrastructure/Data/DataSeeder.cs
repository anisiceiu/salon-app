using SalonApp.Domain.Entities;
using SalonApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SalonApp.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Only seed if database is empty
        if (await context.ServiceCategories.AnyAsync())
        {
            return;
        }

        // Seed Categories
        var categories = new List<ServiceCategory>
        {
            new ServiceCategory
            {
                Name = "Hair",
                Description = "Hair styling and treatment services",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Name = "Facial",
                Description = "Facial and skin care treatments",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Name = "Spa",
                Description = "Full body spa and relaxation treatments",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Name = "Barber",
                Description = "Men's grooming and barber services",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.ServiceCategories.AddRange(categories);
        await context.SaveChangesAsync();

        // Get category IDs
        var hairCategory = categories.First(c => c.Name == "Hair");
        var facialCategory = categories.First(c => c.Name == "Facial");
        var spaCategory = categories.First(c => c.Name == "Spa");
        var barberCategory = categories.First(c => c.Name == "Barber");

        // Seed Services
        var services = new List<Service>
        {
            // Hair services
            new Service
            {
                Name = "Hair Cut",
                Description = "Professional hair cutting and styling",
                Duration = 30,
                Price = 25.00m,
                CategoryId = hairCategory.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Service
            {
                Name = "Hair Color",
                Description = "Full hair coloring service with premium products",
                Duration = 90,
                Price = 80.00m,
                CategoryId = hairCategory.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Barber services
            new Service
            {
                Name = "Beard Trim",
                Description = "Professional beard trimming and shaping",
                Duration = 20,
                Price = 15.00m,
                CategoryId = barberCategory.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Facial services
            new Service
            {
                Name = "Facial",
                Description = "Deep cleansing facial treatment",
                Duration = 45,
                Price = 50.00m,
                CategoryId = facialCategory.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Spa services
            new Service
            {
                Name = "Full Spa",
                Description = "Complete spa package including massage and treatments",
                Duration = 120,
                Price = 120.00m,
                CategoryId = spaCategory.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Services.AddRange(services);
        await context.SaveChangesAsync();

        // Seed Admin User
        var adminUser = new User
        {
            Email = "admin@salon.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "Admin User",
            Phone = "+1234567890",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        // Seed Staff Users
        var staffUsers = new List<User>
        {
            new User
            {
                Email = "john@salon.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff123"),
                FullName = "John Smith",
                Phone = "+1234567891",
                Role = UserRole.Staff,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                Email = "sarah@salon.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff123"),
                FullName = "Sarah Johnson",
                Phone = "+1234567892",
                Role = UserRole.Staff,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                Email = "mike@salon.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff123"),
                FullName = "Mike Davis",
                Phone = "+1234567893",
                Role = UserRole.Staff,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        context.Users.AddRange(staffUsers);
        await context.SaveChangesAsync();

        // Create Staff records
        var hairCutService = services.First(s => s.Name == "Hair Cut");
        var hairColorService = services.First(s => s.Name == "Hair Color");
        var beardTrimService = services.First(s => s.Name == "Beard Trim");
        var facialService = services.First(s => s.Name == "Facial");
        var fullSpaService = services.First(s => s.Name == "Full Spa");

        var staffList = new List<Staff>
        {
            new Staff
            {
                Id = staffUsers[0].Id,
                Bio = "Experienced hair stylist with 10 years of experience in cutting and coloring.",
                ProfileImage = "/images/staff/john.jpg",
                IsAvailable = true
            },
            new Staff
            {
                Id = staffUsers[1].Id,
                Bio = "Professional esthetician specializing in facial treatments and skin care.",
                ProfileImage = "/images/staff/sarah.jpg",
                IsAvailable = true
            },
            new Staff
            {
                Id = staffUsers[2].Id,
                Bio = "Expert barber with specialization in men's grooming and beard styling.",
                ProfileImage = "/images/staff/mike.jpg",
                IsAvailable = true
            }
        };

        context.Staff.AddRange(staffList);
        await context.SaveChangesAsync();

        // Assign services to staff
        var staffServices = new List<StaffService>
        {
            // John - Hair services
            new StaffService { StaffId = staffUsers[0].Id, ServiceId = hairCutService.Id },
            new StaffService { StaffId = staffUsers[0].Id, ServiceId = hairColorService.Id },
            // Sarah - Facial and Spa
            new StaffService { StaffId = staffUsers[1].Id, ServiceId = facialService.Id },
            new StaffService { StaffId = staffUsers[1].Id, ServiceId = fullSpaService.Id },
            // Mike - Barber services
            new StaffService { StaffId = staffUsers[2].Id, ServiceId = beardTrimService.Id },
            new StaffService { StaffId = staffUsers[2].Id, ServiceId = hairCutService.Id }
        };

        context.StaffServices.AddRange(staffServices);
        await context.SaveChangesAsync();

        // Set working hours for staff
        var workingHours = new List<WorkingHours>();

        foreach (var staff in staffList)
        {
            // Monday to Friday
            for (int day = 1; day <= 5; day++)
            {
                workingHours.Add(new WorkingHours
                {
                    StaffId = staff.Id,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    IsWorking = true
                });
            }
            // Saturday
            workingHours.Add(new WorkingHours
            {
                StaffId = staff.Id,
                DayOfWeek = 6,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(16, 0, 0),
                IsWorking = true
            });
            // Sunday - not working
            workingHours.Add(new WorkingHours
            {
                StaffId = staff.Id,
                DayOfWeek = 0,
                StartTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(0, 0, 0),
                IsWorking = false
            });
        }

        context.WorkingHours.AddRange(workingHours);
        await context.SaveChangesAsync();
    }
}
