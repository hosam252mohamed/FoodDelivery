using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models.ViewModels
{
	public class CartViewModel
	{
        public CartViewModel()
        {
            orderHeader = new OrderHeader();
        }
        public List<Cart> CartsList { get; set; }
        public OrderHeader orderHeader { get; set; }
        public ShippingInfo ShippingInfo { get; set; }
    }
}
