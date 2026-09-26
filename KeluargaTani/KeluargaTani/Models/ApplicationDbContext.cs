using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace KeluargaTani.Models;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Pembeli> Pembelis { get; set; }

    public virtual DbSet<Penjualan> Penjualans { get; set; }

    public virtual DbSet<Produk> Produks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=ConnectionStrings:MySqlConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Pembeli>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pembeli");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlamatLahan)
                .HasMaxLength(300)
                .HasColumnName("alamat_lahan");
            entity.Property(e => e.Catatan)
                .HasMaxLength(200)
                .HasColumnName("catatan");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.NamaPembeli)
                .HasMaxLength(50)
                .HasColumnName("nama_pembeli");
            entity.Property(e => e.NomorKontak)
                .HasMaxLength(15)
                .HasColumnName("nomor_kontak");
        });

        modelBuilder.Entity<Penjualan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("penjualan");

            entity.HasIndex(e => e.IdPembeli, "penjualan_pembeli_FK");

            entity.HasIndex(e => e.IdProduk, "penjualan_produk_FK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CatatanBarang)
                .HasMaxLength(100)
                .HasColumnName("catatanBarang");
            entity.Property(e => e.CatatanTambahan)
                .HasMaxLength(200)
                .HasColumnName("catatanTambahan");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.HargaJual)
                .HasPrecision(10)
                .HasColumnName("hargaJual");
            entity.Property(e => e.IdPembeli).HasColumnName("idPembeli");
            entity.Property(e => e.IdProduk).HasColumnName("idProduk");
            entity.Property(e => e.Modal)
                .HasPrecision(10)
                .HasColumnName("modal");
            entity.Property(e => e.NilaiDiskon).HasColumnName("nilaiDiskon");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.TanggalTransaksi)
                .HasColumnType("datetime")
                .HasColumnName("tanggalTransaksi");
            entity.Property(e => e.TipeDiskon)
                .HasMaxLength(100)
                .HasColumnName("tipeDiskon");

            entity.HasOne(d => d.IdPembeliNavigation).WithMany(p => p.Penjualans)
                .HasForeignKey(d => d.IdPembeli)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("penjualan_pembeli_FK");

            entity.HasOne(d => d.IdProdukNavigation).WithMany(p => p.Penjualans)
                .HasForeignKey(d => d.IdProduk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("penjualan_produk_FK");
        });

        modelBuilder.Entity<Produk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("produk");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Kategori)
                .HasMaxLength(100)
                .HasColumnName("kategori");
            entity.Property(e => e.NamaProduk)
                .HasMaxLength(100)
                .HasColumnName("nama_produk");
            entity.Property(e => e.Status)
                .HasComment("1. Tersedia \r\n2. Stock Menipis\r\n3. Habis\r\n4. Non Aktif")
                .HasColumnName("status");
            entity.Property(e => e.Stok).HasColumnName("stok");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
