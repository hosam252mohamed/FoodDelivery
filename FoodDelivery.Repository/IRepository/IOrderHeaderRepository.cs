using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Repository.IRepository
{
    public interface IOrderHeaderRepository
    {
        void Add(OrderHeader orderHeader);
        void UpdatePaymentID(int id, string sessionId, string paymentId);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void Remove(OrderHeader orderHeader);
        OrderHeader Get(int id, string? include = null);
        void Save();
    }
}
