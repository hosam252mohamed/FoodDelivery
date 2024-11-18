using FoodDelivery.Models;
using FoodDelivery.Repository.Data;
using FoodDelivery.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(Category category)
        {
            _context.Add(category);
        }

        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public Category GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryID == id);
        }

        public void Remove(int categoryId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == categoryId);
            if(category != null)
                _context.Categories.Remove(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Remove(Category category)
        {
            _context.Remove(category);
        }
    }
}
