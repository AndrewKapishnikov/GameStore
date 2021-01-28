using GameStore.DataEF;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Data.EF
{
    public class GameStoreDbContext : IdentityDbContext<User>
    {
        public DbSet<GameDTO> Games { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderItemDTO> OrderItems { get; set; }

        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
            : base(options) {  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            BuildCategories(modelBuilder);
            BuildOrderItems(modelBuilder);
            BuildOrders(modelBuilder);
            BuildGames(modelBuilder);
            BuildUsers(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void BuildCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryDTO>(action =>
            {
                action.Property(dto => dto.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });
        }

        private void BuildOrderItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItemDTO>(action =>
            {
                action.Property(dto => dto.Price)
                      .HasColumnType("money")
                      .HasPrecision(15, 2);

                action.HasOne(dto => dto.Order)
                      .WithMany(dto => dto.Items)
                      .IsRequired();

                action.HasOne(dto => dto.Game)
                      .WithMany(dto => dto.OrderItems)
                      .IsRequired(false);
            });

        }

        private static void BuildOrders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDTO>(action =>
            {
                action.HasOne(dto => dto.User)
                      .WithMany(user => user.Orders)
                      .IsRequired(false);
            });
        }


        private static void BuildGames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameDTO>(action =>
            {
                action.Property(dto => dto.Name)
                      .HasMaxLength(30)
                      .IsRequired();

                action.Property(dto => dto.Publisher)
                       .HasMaxLength(40)
                       .IsRequired();

                action.Property(dto => dto.Price)
                      .HasColumnType("money")
                      .HasPrecision(15, 2);

                action.Property(dto => dto.ShortDescription)
                       .HasMaxLength(2000)
                       .IsRequired();

                action.Property(dto => dto.Description)
                      .IsRequired();

                action.Property(dto => dto.ReleaseDate)
                      .HasMaxLength(50)
                      .IsRequired();

                action.HasOne(dto => dto.Category)
                       .WithMany(dto => dto.Games)
                       .IsRequired(false);
            });
        }


        private static void BuildUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(action =>
            {
                action.Property(dto => dto.City)
                 //     .IsRequired()   //?
                      .HasMaxLength(30);
            });
        }



    }




}
