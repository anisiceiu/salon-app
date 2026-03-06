using SalonApp.Domain.Entities;
using System.Threading.Tasks;

namespace SalonApp.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
}
