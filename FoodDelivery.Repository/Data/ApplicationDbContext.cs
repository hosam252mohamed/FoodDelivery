using FoodDelivery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodDelivery.Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        // Existing DbSets
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupun> Coupuns { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ShippingInfo> ShippingInfos { get; set; }
        public DbSet<EmailSubscriber> EmailSubscribers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<CustomerReview> CustomerReviews { get; set; }

    }
}
