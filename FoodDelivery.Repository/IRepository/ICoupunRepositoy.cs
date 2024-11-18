using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository.IRepository
{
    public interface ICoupunRepositoy
    {
        void Add(Coupun coupun);
        void Update(Coupun coupun);
        void Remove(int id);
        List<Coupun> GetAll();
        Coupun Get(int id);
        void Save();
    }
}
