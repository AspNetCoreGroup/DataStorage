﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataStorageCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DataStorageDataAccess.DataBaseContext;

public partial class EnergyContext : DbContext
{
    public EnergyContext(DbContextOptions<EnergyContext> options)
        : base(options)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public virtual DbSet<Archive> Archives { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceType> DeviceTypes { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventDict> EventDicts { get; set; }

    public virtual DbSet<Measurement> Measurements { get; set; }

    public virtual DbSet<MeasurementDict> MeasurementDicts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Archive>(entity =>
        {
            entity.ToTable("archive");

            //entity.Property(e => e.Dt).HasColumnType("datetime");

            entity.HasOne(d => d.Measurument).WithMany(p => p.Archives)
                .HasForeignKey(d => d.MeasurumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_archive_measurement");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("device");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NetAdress)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.SerialNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(d => d.DeviceType).WithMany(p => p.Devices)
                .HasForeignKey(d => d.DeviceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_device_devicetype");
        });

        modelBuilder.Entity<DeviceType>(entity =>
        {
            entity.ToTable("devicetype");
            //entity.
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            List<DeviceType> deviceTypelist = new (); 
            foreach (var devType in CommonTypeDevice.Property.DeviceType.dictionary)
            {
                deviceTypelist.Add(new DeviceType { Id = devType.Key, Name = devType.Value });
            }
            entity.HasData(deviceTypelist);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("event");

            entity.Property(e => e.Id).HasColumnName("id");
            //entity.Property(e => e.Dt).HasColumnType("datetime");

            entity.HasOne(d => d.Device).WithMany(p => p.Events)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_event_device");

            entity.HasOne(d => d.EventDict).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventDictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_event_eventdict");
        });

        modelBuilder.Entity<EventDict>(entity =>
        {
            entity.ToTable("eventdict");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
                

            List<EventDict> eventTypelist = new();
            foreach (var eventType in CommonTypeDevice.Event.EventDictionary.dictionary)
            {
                eventTypelist.Add(new EventDict { Id = eventType.Key, Name = eventType.Value });
            }
            entity.HasData(eventTypelist);
        });

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.ToTable("measurement");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Device).WithMany(p => p.Measurements)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_measurement_device");

            entity.HasOne(d => d.MeasurumentDict).WithMany(p => p.Measurements)
                .HasForeignKey(d => d.MeasurumentDictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_masurement_measurement");
        });

        modelBuilder.Entity<MeasurementDict>(entity =>
        {
            entity.ToTable("measurementdict");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
                

            List<MeasurementDict> measurementDictList = new();
            foreach (var devType in CommonTypeDevice.MeasurumentData.MeasurementDictionary.dictionary)
            {
                measurementDictList.Add(new MeasurementDict { Id = devType.Key, Name = devType.Value });
            }
            entity.HasData(measurementDictList);
        });

        OnModelCreatingPartial(modelBuilder);

        
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}