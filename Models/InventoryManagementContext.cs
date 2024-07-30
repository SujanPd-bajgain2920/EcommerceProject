using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Models;

public partial class InventoryManagementContext : DbContext
{
    public InventoryManagementContext()
    {
    }

    public InventoryManagementContext(DbContextOptions<InventoryManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<BillPrint> BillPrints { get; set; }

    public virtual DbSet<BillRecord> BillRecords { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<UserList> UserLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=Con");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.Bdid).HasName("PK__BillDeta__3EFE4173ABCF6AC7");

            entity.ToTable("BillDetail");

            entity.Property(e => e.Bdid)
                .ValueGeneratedNever()
                .HasColumnName("BDId");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Bid).HasColumnName("BId");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.BidNavigation).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.Bid)
                .HasConstraintName("FK_BillRecord");

            entity.HasOne(d => d.Product).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Product");
        });

        modelBuilder.Entity<BillPrint>(entity =>
        {
            entity.HasKey(e => e.PrintId).HasName("PK__BillPrin__26C7BA7D5E737C41");

            entity.ToTable("BillPrint");

            entity.Property(e => e.PrintId).ValueGeneratedNever();
            entity.Property(e => e.Bid).HasColumnName("BId");

            entity.HasOne(d => d.BidNavigation).WithMany(p => p.BillPrints)
                .HasForeignKey(d => d.Bid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BillRecord_BillPrint");

            entity.HasOne(d => d.PrintByNavigation).WithMany(p => p.BillPrints)
                .HasForeignKey(d => d.PrintBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserList_BillPrint");
        });

        modelBuilder.Entity<BillRecord>(entity =>
        {
            entity.HasKey(e => e.Bid).HasName("PK__BillReco__C6DE0CC10492AD93");

            entity.ToTable("BillRecord");

            entity.Property(e => e.Bid)
                .ValueGeneratedNever()
                .HasColumnName("BId");
            entity.Property(e => e.BillDate).HasColumnType("datetime");
            entity.Property(e => e.Bno).HasColumnName("BNo");
            entity.Property(e => e.CancelDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionType).HasMaxLength(10);

            entity.HasOne(d => d.CancelByUser).WithMany(p => p.BillRecordCancelByUsers)
                .HasForeignKey(d => d.CancelByUserId)
                .HasConstraintName("FK_CancelBy");

            entity.HasOne(d => d.EntryByUser).WithMany(p => p.BillRecordEntryByUsers)
                .HasForeignKey(d => d.EntryByUserId)
                .HasConstraintName("FK_EntryBy");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("PK__Category__6A1C8AFA87D4E396");

            entity.ToTable("Category");

            entity.Property(e => e.CatId).ValueGeneratedNever();
            entity.Property(e => e.CatName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProId).HasName("PK__Product__620295909484BC96");

            entity.ToTable("Product");

            entity.HasIndex(e => e.ProName, "UQ__Product__D695FD1EB34E703E").IsUnique();

            entity.Property(e => e.ProId).ValueGeneratedNever();
            entity.Property(e => e.ProName).HasMaxLength(50);
            entity.Property(e => e.ProPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Product__Categor__4222D4EF");
        });

        modelBuilder.Entity<UserList>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserList__1788CC4CE074958B");

            entity.ToTable("UserList");

            entity.HasIndex(e => e.EmailAddress, "UQ__UserList__49A14740B192F32E").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CurrentAddress).HasMaxLength(50);
            entity.Property(e => e.EmailAddress).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.UserRole).HasMaxLength(40);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
