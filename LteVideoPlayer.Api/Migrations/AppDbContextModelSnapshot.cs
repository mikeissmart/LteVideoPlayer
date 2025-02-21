﻿// <auto-generated />
using System;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LteVideoPlayer.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LteVideoPlayer.Api.Models.Entities.ConvertFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AudioStreamIndex")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DirectoryEnum")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Errored")
                        .HasColumnType("bit");

                    b.Property<string>("Output")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ConvertFiles");
                });

            modelBuilder.Entity("LteVideoPlayer.Api.Models.Entities.ThumbnailError", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DirectoryEnum")
                        .HasColumnType("int");

                    b.Property<string>("Error")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastError")
                        .HasColumnType("datetime2");

                    b.Property<int>("TimesFailed")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ThumbnailErrors");
                });

            modelBuilder.Entity("LteVideoPlayer.Api.Models.Entities.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("LteVideoPlayer.Api.Models.Entities.ConvertFile", b =>
                {
                    b.OwnsOne("LteVideoPlayer.Api.Models.DataTypes.FileDataType", "ConvertedFile", b1 =>
                        {
                            b1.Property<int>("ConvertFileId")
                                .HasColumnType("int");

                            b1.Property<string>("File")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Path")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ConvertFileId");

                            b1.ToTable("ConvertFiles");

                            b1.WithOwner()
                                .HasForeignKey("ConvertFileId");
                        });

                    b.OwnsOne("LteVideoPlayer.Api.Models.DataTypes.FileDataType", "OriginalFile", b1 =>
                        {
                            b1.Property<int>("ConvertFileId")
                                .HasColumnType("int");

                            b1.Property<string>("File")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Path")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ConvertFileId");

                            b1.ToTable("ConvertFiles");

                            b1.WithOwner()
                                .HasForeignKey("ConvertFileId");
                        });

                    b.Navigation("ConvertedFile")
                        .IsRequired();

                    b.Navigation("OriginalFile")
                        .IsRequired();
                });

            modelBuilder.Entity("LteVideoPlayer.Api.Models.Entities.ThumbnailError", b =>
                {
                    b.OwnsOne("LteVideoPlayer.Api.Models.DataTypes.FileDataType", "File", b1 =>
                        {
                            b1.Property<int>("ThumbnailErrorId")
                                .HasColumnType("int");

                            b1.Property<string>("File")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Path")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ThumbnailErrorId");

                            b1.ToTable("ThumbnailErrors");

                            b1.WithOwner()
                                .HasForeignKey("ThumbnailErrorId");
                        });

                    b.Navigation("File")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
