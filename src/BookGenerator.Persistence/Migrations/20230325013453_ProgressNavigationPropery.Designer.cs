﻿// <auto-generated />
using System;
using BookGenerator.Persistence.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookGenerator.Persistence.Migrations
{
    [DbContext(typeof(BookDbContext))]
    [Migration("20230325013453_ProgressNavigationPropery")]
    partial class ProgressNavigationPropery
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BookGenerator.Domain.Core.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BookGenerator.Domain.Core.BookProgress", b =>
                {
                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Progress")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("BookId");

                    b.ToTable("BookProgresses");
                });

            modelBuilder.Entity("BookGenerator.Domain.Core.Chapter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("Chapter");
                });

            modelBuilder.Entity("BookGenerator.Domain.Core.BookProgress", b =>
                {
                    b.HasOne("BookGenerator.Domain.Core.Book", null)
                        .WithOne()
                        .HasForeignKey("BookGenerator.Domain.Core.BookProgress", "BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookGenerator.Domain.Core.Chapter", b =>
                {
                    b.HasOne("BookGenerator.Domain.Core.Book", null)
                        .WithMany("Chapters")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookGenerator.Domain.Core.Book", b =>
                {
                    b.Navigation("Chapters");
                });
#pragma warning restore 612, 618
        }
    }
}
