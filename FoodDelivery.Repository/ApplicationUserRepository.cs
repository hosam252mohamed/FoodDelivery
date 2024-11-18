using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using FoodDelivery.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ApplicationUserRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Add(ApplicationUser user, string password)
        {
            _userManager.CreateAsync(user,password).GetAwaiter().GetResult();
        }

        public ApplicationUser Get(string id)
        {
            return _context.ApplicationUsers.FirstOrDefault(u=>u.Id == id);
        }

        public List<ApplicationUser> GetAll(ApplicationUser user)
        {
            return _context.ApplicationUsers.Where(u => u.Id != user.Id).ToList();
        }

        public void Remove(ApplicationUser user)
        {
            _userManager.DeleteAsync(user).GetAwaiter().GetResult();
        }

        public void Update(ApplicationUser user,string newRole)
        {
            string role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().First();
            _userManager.RemoveFromRoleAsync(user,role).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, newRole).GetAwaiter().GetResult();
            user.Role = newRole;
            _context.Update(user);
            _context.SaveChanges();
        }
    }
}
