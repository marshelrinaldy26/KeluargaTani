using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KeluargaTani.Models;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql("name=ConnectionStrings:MySqlConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));
        }
    }

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
