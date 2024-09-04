//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;


using FinalProject.Models;
using Microsoft.AspNetCore.Identity;//
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
 

namespace FinalProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<RegistrationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        #region DbSet
        public virtual DbSet<Cashier> Cashiers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DeliveryBoy> DeliveryBoys { get; set; }
        public virtual DbSet<Extra> Extras { get; set; }
        public virtual DbSet<Meal> Meals { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<OrderMeal> OrderMeals { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Role>().HasData(
        //               new Role { Id = 1, Name = "Admin" },
        //               new Role { Id = 2, Name = "User" }
        //      );



        }

         }

    }
