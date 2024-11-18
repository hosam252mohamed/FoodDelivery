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
    public class OrderHeaderRepository : IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(OrderHeader orderHeader)
        {
            _context.Add(orderHeader);
        }

        public void UpdatePaymentID(int id, string sessionId, string paymentId)
        {
            var orderHeader = Get(id);
            if (sessionId != null) orderHeader.SessionId = sessionId;
            if (paymentId != null) orderHeader.TransactionId = paymentId;
        }

        public void Remove(OrderHeader orderHeader)
        {
            _context.Remove(orderHeader);
        }

        public OrderHeader Get(int id, string? include = null)
        {
            if (include == "User") return _context.OrderHeaders.Include(o => o.ApplicationUser).FirstOrDefault(o => o.Id == id);
            return _context.OrderHeaders.FirstOrDefault(o => o.Id == id);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderHeader = Get(id);
            if(orderStatus != null) orderHeader.OrderStatus = orderStatus;
            if (paymentStatus != null) orderHeader.PaymentStatus = paymentStatus;
        }
    }
}
