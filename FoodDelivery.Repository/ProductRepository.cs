using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using FoodDelivery.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            _context.Products.Update(product);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public void Delete(int id)
        {
            _context.Products.Remove(_context.Products.First(p => p.ProductID == id));
        }

        public Product Get(int id)
        {
            return _context.Products.FirstOrDefault(p=>p.ProductID == id);
        }

        public List<Product> GetAll(string Include = null)
        {
            if(Include == "Category")
            {
                return _context.Products.Include(x => x.Category).ToList();
            }
            else if(Include == "ProductImages")
            {
                return _context.Products.Include(x => x.ProductImages).ToList();
            }

            return _context.Products.ToList();
        }
    }
}
