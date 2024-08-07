﻿// <auto-generated />
using System;
using MeterService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MeterService.Migrations
{
    [DbContext(typeof(MeterContext))]
    partial class MeterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MeterService.Models.MeterReading", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Current")
                        .HasColumnType("float");

                    b.Property<double>("EndIndex")
                        .HasColumnType("float");

                    b.Property<DateTime>("ReadingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<double>("Voltage")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("MeterReadings");
                });
#pragma warning restore 612, 618
        }
    }
}
