﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Radzinsky.Persistence;

#nullable disable

namespace Radzinsky.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Radzinsky.Domain.Models.Entities.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("Radzinsky.Domain.Models.Entities.ChatPortal", b =>
                {
                    b.Property<long>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ChatId"));

                    b.Property<int>("MemberCount")
                        .HasColumnType("integer");

                    b.HasKey("ChatId");

                    b.ToTable("ChatPortals");
                });

            modelBuilder.Entity("Radzinsky.Domain.Models.Entities.State", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.ToTable("States");

                    b.HasDiscriminator<string>("Discriminator").HasValue("State");
                });

            modelBuilder.Entity("Radzinsky.Domain.Models.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Bio")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Radzinsky.Domain.Models.Entities.States.SurveyState", b =>
                {
                    b.HasBaseType("Radzinsky.Domain.Models.Entities.State");

                    b.Property<int?>("MatrixCellId")
                        .HasColumnType("integer");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("SurveyState");
                });
#pragma warning restore 612, 618
        }
    }
}
