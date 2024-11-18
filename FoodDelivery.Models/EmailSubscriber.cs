using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
	public class EmailSubscriber
	{
        [Key]
        public int Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
