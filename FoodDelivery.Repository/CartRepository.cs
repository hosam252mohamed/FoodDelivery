using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using FoodDelivery.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(Cart cart)
        {
            _context.Add(cart);
        }
        public void Delete(int id)
        {
            _context.Remove(Get(id));
        }
        public Cart Get(int id, string include = null)
        {

            if (include == "Product")
                return _context.Carts.Include(c => c.Product).Include(c=>c.Product.ProductImages).Include(c=>c.Product.Category).FirstOrDefault(c => c.CartID == id);
            return _context.Carts.FirstOrDefault(c => c.CartID == id);
        }
        public List<Cart> GetAll(string userId, string include = null)
        {
            if (include == "Product")
                return _context.Carts.Where(c=>c.ApplicationUserId == userId).Include(c => c.Product).Include(c => c.Product.ProductImages).Include(c=>c.Product.Category).ToList();
            return _context.Carts.Where(c=>c.ApplicationUserId == userId).ToList();
        }
        public void Update(Cart cart)
        {
            _context.Update(cart);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public decimal CalculateTotalPrice(string userId)
        {
            var CartsList = _context.Carts.Where(c => c.ApplicationUserId == userId).Include(c => c.Product).Include(c => c.Product.ProductImages).Include(c => c.Product.Category).ToList();
            decimal Total = 0;
            if (CartsList is not null)
            {
                foreach (var cart in CartsList)
                {
                    Total += (cart.Product.Price * cart.Count);
                }
                return Total;
            }
            return 0;
        }
    }
}
