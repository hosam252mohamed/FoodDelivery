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
    public class CoupunRepository : ICoupunRepositoy
    {
        private readonly ApplicationDbContext _context;

        public CoupunRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(Coupun coupun)
        {
            _context.Add(coupun);
        }

        public void Update(Coupun coupun)
        {
            _context.Update(coupun);
        }
        public void Remove(int id)
        {
            _context.Remove(_context.Coupuns.FirstOrDefault(c => c.CoupunID == id));;
        }
        public Coupun Get(int id)
        {
            return _context.Coupuns.FirstOrDefault(c => c.CoupunID == id);
        }
        public List<Coupun> GetAll()
        {
            return _context.Coupuns.ToList();
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
