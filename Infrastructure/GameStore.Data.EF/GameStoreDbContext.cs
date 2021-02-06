using GameStore.DataEF;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                      .HasColumnType("decimal")
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

                //Delivery
                action.Property(dto => dto.DeliveryName)
                      .HasMaxLength(50);

                action.Property(dto => dto.DeliveryDescription)
                    .HasMaxLength(1000);

                action.Property(dto => dto.DeliveryPrice)
                      .HasColumnType("decimal")
                      .HasPrecision(15, 2);

                action.Property(dto => dto.DeliveryParameters)
                      .HasConversion(
                          value => JsonConvert.SerializeObject(value),
                          value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value))
                      .Metadata.SetValueComparer(DictionaryComparer);
                //Delivery
            });
        }

        private static readonly ValueComparer DictionaryComparer =
          new ValueComparer<Dictionary<string, string>>(
              (value1, value2) => value1.SequenceEqual(value2),
              dictionary => dictionary.Aggregate(
                  0, (a, b) => HashCode.Combine(HashCode.Combine(a, b.Key.GetHashCode()), b.Value.GetHashCode())
              )
          );

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
                      .HasColumnType("decimal")
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
                action.Property(dto => dto.Name)
                      .IsRequired()
                      .HasMaxLength(30);

                action.Property(dto => dto.Surname)
                      .IsRequired()
                      .HasMaxLength(30);

                action.Property(dto => dto.Address)
                     .IsRequired()
                     .HasMaxLength(50);

                action.Property(dto => dto.City)
                      .IsRequired()  
                      .HasMaxLength(30);
            });
        }



    }




}
