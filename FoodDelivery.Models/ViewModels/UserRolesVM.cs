using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models.ViewModels
{
    public class UserRolesVM
    {
        public ApplicationUser User { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
