using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using FoodDelivery.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public DbInitializer(ApplicationDbContext context,UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void Initialize()
        {
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                context.Database.Migrate();
            }

            if (context.Roles.Any(r => r.Name == "Manager")) return;
            else
            {
                roleManager.CreateAsync(new IdentityRole("Manager")).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "Admin@Food.com",
                    Email = "Admin@Food.com",
                    Address = "Address1",
                    City = "Cairo",
                    PostalCode = "15161"
                };
                userManager.CreateAsync(user,"Admin@123").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
        }
    }
}
