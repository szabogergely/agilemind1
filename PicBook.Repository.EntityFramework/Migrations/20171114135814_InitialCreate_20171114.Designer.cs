﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using PicBook.Repository.EntityFramework;
using System;

namespace PicBook.Repository.EntityFramework.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20171114135814_InitialCreate_20171114")]
    partial class InitialCreate_20171114
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PicBook.Domain.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("ImageIdentifier");

                    b.Property<string>("ImageURL");

                    b.Property<string>("Name");

                    b.Property<bool>("Remote");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<string>("UserIdentifier");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("PicBook.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<string>("UserIdentifier");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}