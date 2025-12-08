using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public  Task<User?> GetByEmailAsync(string email)
        {
            return  _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public  Task<bool> EmailExistsAsync(string email)
        {
            return  _context.Users
                .AnyAsync(u => u.Email == email);
        }
    }
}
