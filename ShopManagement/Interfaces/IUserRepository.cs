using ShopManagement.Models.Entities;
using System.Threading.Tasks;
namespace ShopManagement.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }
}
