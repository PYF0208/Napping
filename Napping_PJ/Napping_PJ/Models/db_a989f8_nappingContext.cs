using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Napping_PJ.Models
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
        public virtual DbSet<Level> Levels { get; set; } = null!;
        public virtual DbSet<Like> Likes { get; set; } = null!;
        public virtual DbSet<Oauth> Oauths { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderDetailExtraService> OrderDetailExtraServices { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Profit> Profits { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomImage> RoomImages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=SQL8005.site4now.net;User ID=db_a989f8_napping_admin;Password=team3team3;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BellEvent>(entity =>
            {
                entity.ToTable("Bell_Event");

                entity.Property(e => e.BellEventId).HasColumnName("Bell_Event_Id");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.EventContent)
                    .HasMaxLength(50)
                    .HasColumnName("Event_Content");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BellEvents)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bell_Event_Customer");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.CommentId).HasColumnName("Comment_Id");

                entity.Property(e => e.Cp).HasColumnName("CP");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.HotelId).HasColumnName("Hotel_Id");

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
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.LevelId).HasColumnName("Level_Id");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Password_Hash");

                entity.Property(e => e.Phone).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Region).HasMaxLength(50);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Level");
            });

            modelBuilder.Entity<CustomerGift>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.GiftId, e.PostDate })
                    .HasName("PK_Gift");

                entity.ToTable("Customer_Gift");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.GiftId).HasColumnName("Gift_Id");

                entity.Property(e => e.PostDate)
                    .HasColumnType("date")
                    .HasColumnName("Post_Date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("End_Date");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("Start_Date");

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
                entity.ToTable("Extra_Service");

                entity.Property(e => e.ExtraServiceId).HasColumnName("Extra_Service_Id");

                entity.Property(e => e.HotelId).HasColumnName("Hotel_Id");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.ExtraServices)
                    .HasForeignKey(d => d.HotelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Extra_Service_Hotel");
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.ToTable("Feature");

                entity.Property(e => e.FeatureId).HasColumnName("Feature_Id");

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Gift>(entity =>
            {
                entity.ToTable("Gift");

                entity.Property(e => e.GiftId).HasColumnName("Gift_Id");

                entity.Property(e => e.HotelId).HasColumnName("Hotel_Id");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.ValidDate).HasColumnName("Valid_Date");
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable("Hotel");

                entity.Property(e => e.HotelId).HasColumnName("Hotel_Id");

                entity.Property(e => e.AvgComment).HasColumnName("Avg_Comment");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.ContactName)
                    .HasMaxLength(50)
                    .HasColumnName("Contact_Name");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Region).HasMaxLength(50);

                entity.Property(e => e.Star)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("Level");

                entity.Property(e => e.LevelId).HasColumnName("Level_Id");

                entity.Property(e => e.LevelName)
                    .HasMaxLength(50)
                    .HasColumnName("Level_Name");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.CustomerId });

                entity.ToTable("Like");

                entity.Property(e => e.RoomId).HasColumnName("Room_Id");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.EndTime)
                    .HasColumnType("date")
                    .HasColumnName("End_Time");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Like_Customer");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Like_Room");
            });

            modelBuilder.Entity<Oauth>(entity =>
            {
                entity.ToTable("OAuth");

                entity.Property(e => e.OauthId)
                    .HasMaxLength(50)
                    .HasColumnName("OAuth_Id");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Oauths)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OAuth_Customer");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.Currency)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.PaymentId).HasColumnName("Payment_Id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Customer");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Payment");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OidRidPid);

                entity.ToTable("Order_Detail");

                entity.Property(e => e.OidRidPid).HasColumnName("OIdRIdPId");

                entity.Property(e => e.CheckIn)
                    .HasColumnType("date")
                    .HasColumnName("Check_In");

                entity.Property(e => e.CheckOut)
                    .HasColumnType("date")
                    .HasColumnName("Check_Out");

                entity.Property(e => e.NumberOfGuests).HasColumnName("Number_Of_Guests");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.ProfitId).HasColumnName("Profit_Id");

                entity.Property(e => e.RoomId).HasColumnName("Room_Id");

                entity.Property(e => e.TravelType)
                    .HasMaxLength(50)
                    .HasColumnName("Travel_Type");

                entity.HasOne(d => d.Profit)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProfitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Detail_Order");

                entity.HasOne(d => d.ProfitNavigation)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProfitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Detail_Profit");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Detail_Room");
            });

            modelBuilder.Entity<OrderDetailExtraService>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("OrderDetail_ExtraService");

                entity.Property(e => e.ExtraServiceId).HasColumnName("Extra_Service_Id");

                entity.Property(e => e.OidRidPid).HasColumnName("OIdRIdPId");

                entity.HasOne(d => d.ExtraService)
                    .WithMany()
                    .HasForeignKey(d => d.ExtraServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_ExtraService_Extra_Service");

                entity.HasOne(d => d.OidRidP)
                    .WithMany()
                    .HasForeignKey(d => d.OidRidPid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_ExtraService_Order_Detail");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId).HasColumnName("Payment_Id");

                entity.Property(e => e.Currency)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Order");
            });

            modelBuilder.Entity<Profit>(entity =>
            {
                entity.ToTable("Profit");

                entity.Property(e => e.ProfitId).HasColumnName("Profit_Id");

                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("Promotion");

                entity.Property(e => e.PromotionId).HasColumnName("Promotion_Id");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("End_Date");

                entity.Property(e => e.LevelId).HasColumnName("Level_Id");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("Start_Date");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Promotion_Level");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.RoomId).HasColumnName("Room_Id");

                entity.Property(e => e.HotelId).HasColumnName("Hotel_Id");

                entity.Property(e => e.MaxGuests).HasColumnName("Max_Guests");

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
                            j.HasKey("RoomId", "FeatureId").HasName("PK_Feature");

                            j.ToTable("Room_Feature");

                            j.IndexerProperty<int>("RoomId").HasColumnName("Room_Id");

                            j.IndexerProperty<int>("FeatureId").HasColumnName("Feature_Id");
                        });
            });

            modelBuilder.Entity<RoomImage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Room_Image");

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.RoomId).HasColumnName("Room_Id");

                entity.Property(e => e.RoomImageId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Room_Image_Id");

                entity.HasOne(d => d.Room)
                    .WithMany()
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Room_Image_Room");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
