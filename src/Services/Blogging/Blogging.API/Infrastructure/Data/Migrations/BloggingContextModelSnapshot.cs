﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Blogging.API.Infrastructure.Data.Migrations
{
    [DbContext(typeof(BloggingContext))]
    partial class BloggingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("blogging")
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Blogging.API.Infrastructure.Data.Entities.Blog", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(36)
                    .HasColumnType("nvarchar(36)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DisabledAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Footer")
                    .HasMaxLength(1000)
                    .HasColumnType("nvarchar(1000)");

                b.Property<string>("Heading")
                    .HasMaxLength(1000)
                    .HasColumnType("nvarchar(1000)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<string>("OwnerId")
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnType("nvarchar(36)");

                b.Property<string>("Path")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<DateTime?>("SoftDeletedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Title")
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Uri")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.HasKey("Id");

                b.ToTable("Blogs");
            });

            modelBuilder.Entity("Blogging.API.Infrastructure.Data.Entities.DataMigration", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(36)
                    .HasColumnType("nvarchar(36)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.HasKey("Id");

                b.ToTable("DataMigrations");
            });
#pragma warning restore 612, 618
        }
    }
}