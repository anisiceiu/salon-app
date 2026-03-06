using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;

namespace SalonApp.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IServiceCategoryRepository _categoryRepository;

    public ServiceService(IServiceRepository serviceRepository, IServiceCategoryRepository categoryRepository)
    {
        _serviceRepository = serviceRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await _serviceRepository.GetAllAsync();
        return services.Select(MapToServiceDto);
    }

    public async Task<ServiceDto?> GetServiceByIdAsync(int id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        return service == null ? null : MapToServiceDto(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(int categoryId)
    {
        var services = await _serviceRepository.GetByCategoryAsync(categoryId);
        return services.Select(MapToServiceDto);
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceRequest request)
    {
        ValidateServiceRequest(request);

        // Check if category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Price = request.Price,
            CategoryId = request.CategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _serviceRepository.AddAsync(service);
        return MapToServiceDto(created);
    }

    public async Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceRequest request)
    {
        ValidateUpdateServiceRequest(request);

        var existingService = await _serviceRepository.GetByIdAsync(id);
        if (existingService == null)
        {
            throw new KeyNotFoundException("Service not found");
        }

        // Check if category exists if changed
        if (request.CategoryId != existingService.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Category not found");
            }
        }

        existingService.Name = request.Name;
        existingService.Description = request.Description;
        existingService.Duration = request.Duration;
        existingService.Price = request.Price;
        existingService.CategoryId = request.CategoryId;
        existingService.IsActive = request.IsActive;

        var updated = await _serviceRepository.UpdateAsync(existingService);
        return MapToServiceDto(updated);
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service == null)
        {
            return false;
        }

        // Soft delete - set IsActive to false
        service.IsActive = false;
        await _serviceRepository.UpdateAsync(service);
        return true;
    }

    public async Task<IEnumerable<ServiceCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToCategoryDto);
    }

    public async Task<ServiceCategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToCategoryDto(category);
    }

    public async Task<ServiceCategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Category name is required");
        }

        var category = new ServiceCategory
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _categoryRepository.AddAsync(category);
        return MapToCategoryDto(created);
    }

    public async Task<ServiceCategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Category name is required");
        }

        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;
        existingCategory.IsActive = request.IsActive;

        var updated = await _categoryRepository.UpdateAsync(existingCategory);
        return MapToCategoryDto(updated);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        // Soft delete - set IsActive to false
        category.IsActive = false;
        await _categoryRepository.UpdateAsync(category);
        return true;
    }

    private static void ValidateServiceRequest(CreateServiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Service name is required");
        }

        if (request.Duration <= 0)
        {
            throw new ArgumentException("Duration must be greater than 0");
        }

        if (request.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }
    }

    private static void ValidateUpdateServiceRequest(UpdateServiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Service name is required");
        }

        if (request.Duration <= 0)
        {
            throw new ArgumentException("Duration must be greater than 0");
        }

        if (request.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }
    }

    private static ServiceDto MapToServiceDto(Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Duration = service.Duration,
            Price = service.Price,
            CategoryId = service.CategoryId,
            CategoryName = service.Category?.Name,
            IsActive = service.IsActive,
            CreatedAt = service.CreatedAt
        };
    }

    private static ServiceCategoryDto MapToCategoryDto(ServiceCategory category)
    {
        return new ServiceCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }
}
