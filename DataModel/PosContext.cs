using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataModel;

public partial class PosContext : DbContext
{
    public PosContext()
    {
    }

    public PosContext(DbContextOptions<PosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCoupon> TblCoupons { get; set; }

    public virtual DbSet<TblExchangeCoupon> TblExchangeCoupons { get; set; }

    public virtual DbSet<TblItem> TblItems { get; set; }

    public virtual DbSet<TblMember> TblMembers { get; set; }

    public virtual DbSet<TblPurchaseHistory> TblPurchaseHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseMySql("server=127.0.0.1,3306;database=POS;user=root;password=Htetsuaung@1", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.1.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<TblCoupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PRIMARY");

            entity.ToTable("tbl_Coupon");

            entity.HasIndex(e => e.CouponNo, "Coupon_No_UNIQUE").IsUnique();

            entity.Property(e => e.CouponId).HasColumnName("Coupon_ID");
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");
            entity.Property(e => e.Amount).HasPrecision(20, 2);
            entity.Property(e => e.CouponName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Coupon_Name");
            entity.Property(e => e.CouponNo)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnName("Coupon_No");
            entity.Property(e => e.AvailableQty).HasColumnName("Available_Qty");
        });

        modelBuilder.Entity<TblExchangeCoupon>(entity =>
        {
            entity.HasKey(e => e.ExchangeCouponId).HasName("PRIMARY");

            entity.ToTable("tbl_Exchange_Coupon");

            entity.HasIndex(e => e.CouponId, "Coupon_ID_Ex_idx");

            entity.HasIndex(e => e.MemberId, "Member_ID_Ex_idx");

            entity.Property(e => e.ExchangeCouponId).HasColumnName("Exchange_Coupon_ID");
            entity.Property(e => e.CouponId).HasColumnName("Coupon_ID");
            entity.Property(e => e.IsUsed).HasColumnName("Is_Used");
            entity.Property(e => e.MemberId).HasColumnName("Member_ID");

            entity.HasOne(d => d.Coupon).WithMany(p => p.TblExchangeCoupons)
                .HasForeignKey(d => d.CouponId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Coupon_ID_Ex");

            entity.HasOne(d => d.Member).WithMany(p => p.TblExchangeCoupons)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Member_ID_Ex");
        });

        modelBuilder.Entity<TblItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PRIMARY");

            entity.ToTable("tbl_Item");

            entity.HasIndex(e => e.ItemCode, "Item_Code_UNIQUE").IsUnique();

            entity.Property(e => e.ItemId).HasColumnName("Item_ID");
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");
            entity.Property(e => e.ItemCode)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnName("Item_Code");
            entity.Property(e => e.ItemName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Item_Name");
            entity.Property(e => e.Price).HasPrecision(20, 2);
        });

        modelBuilder.Entity<TblMember>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PRIMARY");

            entity.ToTable("tbl_Member");

            entity.HasIndex(e => e.MemberCode, "Member_Code_UNIQUE").IsUnique();

            entity.HasIndex(e => e.MobileNumber, "Mobile_Number_UNIQUE").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("Member_ID");
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.MemberCode)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnName("Member_Code");
            entity.Property(e => e.MemberName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Member_Name");
            entity.Property(e => e.MobileNumber)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnName("Mobile_Number");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(10)
                .HasColumnName("Otp_Code");
            entity.Property(e => e.TotalPoints).HasColumnName("Total_Points");
        });

        modelBuilder.Entity<TblPurchaseHistory>(entity =>
        {
            entity.HasKey(e => e.PurchaseHistoryId).HasName("PRIMARY");

            entity.ToTable("tbl_Purchase_History");

            entity.HasIndex(e => e.InvoiceNo, "Invoice_No_UNIQUE").IsUnique();

            entity.HasIndex(e => e.MemberId, "Member_ID_idx");

            entity.Property(e => e.PurchaseHistoryId).HasColumnName("Purchase_History_ID");
            entity.Property(e => e.Amount).HasPrecision(20, 2);
            entity.Property(e => e.ExchangeCouponId).HasColumnName("Exchange_Coupon_ID");
            entity.Property(e => e.InvoiceNo)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnName("Invoice_No");
            entity.Property(e => e.MemberId).HasColumnName("Member_ID");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("datetime")
                .HasColumnName("Purchase_Date");

            entity.HasOne(d => d.Member).WithMany(p => p.TblPurchaseHistories)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Member_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
