using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository.IRepository
{
    public interface IApplicationUserRepository
    {
        void Add(ApplicationUser user, string password);
        void Update(ApplicationUser user, string role);
        void Remove(ApplicationUser user);
        List<ApplicationUser> GetAll(ApplicationUser user);
        ApplicationUser Get(string id);
    }
}
