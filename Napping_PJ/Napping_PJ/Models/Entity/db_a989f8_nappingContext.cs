﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Napping_PJ.Models.Entity
{
    public partial class db_a989f8_nappingContext : DbContext
    {
        public db_a989f8_nappingContext()
        {
        }

        public db_a989f8_nappingContext(DbContextOptions<db_a989f8_nappingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BellEvent> BellEvents { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<CustomerGift> CustomerGifts { get; set; } = null!;
        public virtual DbSet<ExtraService> ExtraServices { get; set; } = null!;
        public virtual DbSet<Feature> Features { get; set; } = null!;
        public virtual DbSet<Gift> Gifts { get; set; } = null!;
        public virtual DbSet<Hotel> Hotels { get; set; } = null!;
        public virtual DbSet<HotelExtraService> HotelExtraServices { get; set; } = null!;
        public virtual DbSet<Level> Levels { get; set; } = null!;
        public virtual DbSet<Like> Likes { get; set; } = null!;
        public virtual DbSet<Oauth> Oauths { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderDetailExtraService> OrderDetailExtraServices { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Profit> Profits { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomImage> RoomImages { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:napping-jp.database.windows.net,1433;Initial Catalog=db_a989f8_napping;Persist Security Info=False;User ID=NappingSa;Password=zaq!xsw2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BellEvent>(entity =>
            {
                entity.Property(e => e.EventContent).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BellEvents)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bell_Event_Customer");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Cp).HasColumnName("CP");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Customer");

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.HotelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Hotel");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Locked)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Region).HasMaxLength(50);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK_Customer_Level");
            });

            modelBuilder.Entity<CustomerGift>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.GiftId, e.PostDate })
                    .HasName("PK_Gift");

                entity.Property(e => e.PostDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerGifts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Gift_Customer");

                entity.HasOne(d => d.Gift)
                    .WithMany(p => p.CustomerGifts)
                    .HasForeignKey(d => d.GiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Gift_Gift");
            });

            modelBuilder.Entity<ExtraService>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Gift>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.ContactName).HasMaxLength(50);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Region).HasMaxLength(50);

                entity.Property(e => e.Star)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<HotelExtraService>(entity =>
            {
                entity.HasKey(e => new { e.HotelId, e.ExtraServiceId })
                    .HasName("PK_HotelExtraServices_1");

                entity.HasOne(d => d.ExtraService)
                    .WithMany(p => p.HotelExtraServices)
                    .HasForeignKey(d => d.ExtraServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HotelExtraServices_ExtraServices");

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.HotelExtraServices)
                    .HasForeignKey(d => d.HotelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HotelExtraServices_Hotels");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(e => new { e.HotelId, e.CustomerId })
                    .HasName("PK_Like");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Like_Customer");

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.HotelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Likes_Hotels");
            });

            modelBuilder.Entity<Oauth>(entity =>
            {
                entity.ToTable("OAuths");

                entity.Property(e => e.OauthId)
                    .HasMaxLength(50)
                    .HasColumnName("OAuthId");

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Oauths)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OAuth_Customer");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.NameOfBooking)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneOfBooking)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Customers");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.CheckIn).HasColumnType("datetime");

                entity.Property(e => e.CheckOut).HasColumnType("datetime");

                entity.Property(e => e.TravelType).HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Rooms");
            });

            modelBuilder.Entity<OrderDetailExtraService>(entity =>
            {
                entity.HasKey(e => e.Odesid)
                    .HasName("PK_OrderDetailExtraServices_1");

                entity.Property(e => e.Odesid).HasColumnName("ODESId");

                entity.Property(e => e.EstotalPrice).HasColumnName("ESTotalPrice");

                entity.Property(e => e.ExtraServiceName).HasMaxLength(50);

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.OrderDetailExtraServices)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_ExtraService_Order_Detail");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payments_Orders");
            });

            modelBuilder.Entity<Profit>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Promotion_Level");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.HotelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Room_Hotel");

                entity.HasMany(d => d.Features)
                    .WithMany(p => p.Rooms)
                    .UsingEntity<Dictionary<string, object>>(
                        "RoomFeature",
                        l => l.HasOne<Feature>().WithMany().HasForeignKey("FeatureId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Room_Feature_Feature"),
                        r => r.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Room_Feature_Room"),
                        j =>
                        {
                            j.HasKey("RoomId", "FeatureId").HasName("PK_RoomFeatures_1");

                            j.ToTable("RoomFeatures");
                        });
            });

            modelBuilder.Entity<RoomImage>(entity =>
            {
                entity.Property(e => e.Image).IsUnicode(false);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomImages)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Room_Image_Room");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleCustomerId);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Customer");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Roles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
