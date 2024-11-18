using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository.IRepository
{
    public interface ICartRepository
    {
        void Add(Cart cart);
        void Update(Cart cart);
        void Delete(int id);
        Cart Get(int id, string include = null);
        List<Cart> GetAll(string userId, string include = null);
        void Save();
        decimal CalculateTotalPrice(string userId);
    }
}
