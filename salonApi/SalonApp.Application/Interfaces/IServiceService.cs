using SalonApp.Application.DTOs;

namespace SalonApp.Application.Interfaces;

public interface IServiceService
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    Task<ServiceDto?> GetServiceByIdAsync(int id);
    Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(int categoryId);
    Task<ServiceDto> CreateServiceAsync(CreateServiceRequest request);
    Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceRequest request);
    Task<bool> DeleteServiceAsync(int id);
    
    Task<IEnumerable<ServiceCategoryDto>> GetAllCategoriesAsync();
    Task<ServiceCategoryDto?> GetCategoryByIdAsync(int id);
    Task<ServiceCategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
    Task<ServiceCategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(int id);
}
