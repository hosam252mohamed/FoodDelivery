using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository.IRepository
{
    public interface ICategoryRepository
    {
        void Add(Category category);
        void Update(Category category);
        void Remove(int categoryId); 
        void Remove(Category category);
        List<Category> GetAll();
        Category GetById(int id);
        void Save();
    }
}
