using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AdminDAL.Entities2;

public partial class AdminCont : DbContext
{
    public AdminCont()
    {
    }

    public AdminCont(DbContextOptions<AdminCont> options)
        : base(options)
    {
    }

    public virtual DbSet<EntityTbl> EntityTbls { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<UserAdminRole> UserAdminRoles { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityTbl>(entity =>
        {
            entity.HasKey(e => e.EntityName);

            entity.ToTable("EntityTbl");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasIndex(e => e.EntityName, "IX_Features_EntityName");

            entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
            entity.Property(e => e.UserName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.EntityNameNavigation).WithMany(p => p.Features).HasForeignKey(d => d.EntityName);
        });

        modelBuilder.Entity<UserAdminRole>(entity =>
        {
            entity.ToTable("UserAdminRole");

            entity.Property(e => e.Id).HasColumnName("ID");
        });
        modelBuilder.HasSequence<int>("SalesOrderNumber", "SalesLT");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
