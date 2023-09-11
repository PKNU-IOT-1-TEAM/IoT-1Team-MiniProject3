using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarReservationWeb.Models;

public partial class Team1IotContext : DbContext
{
    public Team1IotContext()
    {
    }

    public Team1IotContext(DbContextOptions<Team1IotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountParking> AccountParkings { get; set; }

    public virtual DbSet<ParkingHistory> ParkingHistories { get; set; }

    public virtual DbSet<ParkingStatus> ParkingStatuses { get; set; }

    public virtual DbSet<SensorDb> SensorDbs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=210.119.12.100;port=10000;database=team1_iot;user=pi;password=12345", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AccountParking>(entity =>
        {
            entity.HasKey(e => e.IdX).HasName("PRIMARY");

            entity.ToTable("account_parking");

            entity.Property(e => e.IdX).HasColumnName("id_x");
            entity.Property(e => e.Admin)
                .HasDefaultValueSql("'1'")
                .HasColumnName("admin");
            entity.Property(e => e.CarNumber)
                .HasMaxLength(100)
                .HasColumnName("car_number")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .HasColumnName("id");
            entity.Property(e => e.NfcRegistered)
                .HasMaxLength(100)
                .HasColumnName("nfc_registered");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
        });

        modelBuilder.Entity<ParkingHistory>(entity =>
        {
            entity.HasKey(e => e.IdX).HasName("PRIMARY");

            entity.ToTable("parking_history");

            entity.Property(e => e.IdX)
                .ValueGeneratedNever()
                .HasColumnName("id_x");
            entity.Property(e => e.DeparureTime)
                .HasColumnType("datetime")
                .HasColumnName("deparure_time");
            entity.Property(e => e.EntranceTime)
                .HasColumnType("datetime")
                .HasColumnName("entrance_time");
            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .HasColumnName("id");
        });

        modelBuilder.Entity<ParkingStatus>(entity =>
        {
            entity.HasKey(e => e.IdX).HasName("PRIMARY");

            entity.ToTable("parking_status");

            entity.Property(e => e.IdX).HasColumnName("id_x");
            entity.Property(e => e.Nfc)
                .HasMaxLength(100)
                .HasColumnName("NFC");
            entity.Property(e => e.ParkingIr).HasColumnName("parking_IR");
            entity.Property(e => e.ReservationStatus)
                .HasMaxLength(100)
                .HasColumnName("reservation_status")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<SensorDb>(entity =>
        {
            entity.HasKey(e => e.IdX).HasName("PRIMARY");

            entity.ToTable("sensor_db");

            entity.Property(e => e.IdX)
                .ValueGeneratedNever()
                .HasColumnName("id_x");
            entity.Property(e => e.Ad1RcvDust).HasColumnName("AD1_RCV_Dust");
            entity.Property(e => e.Ad1RcvHumidity).HasColumnName("AD1_RCV_Humidity");
            entity.Property(e => e.Ad1RcvIrSensor).HasColumnName("AD1_RCV_IR_Sensor");
            entity.Property(e => e.Ad1RcvParkingStatus)
                .HasMaxLength(100)
                .HasColumnName("AD1_RCV_Parking_Status");
            entity.Property(e => e.Ad1RcvTemperature).HasColumnName("AD1_RCV_Temperature");
            entity.Property(e => e.Ad2RcvCguard).HasColumnName("AD2_RCV_CGuard");
            entity.Property(e => e.Ad3RcvWguardWave).HasColumnName("AD3_RCV_WGuard_WAVE");
            entity.Property(e => e.Ad4RcvNfc)
                .HasMaxLength(100)
                .HasColumnName("AD4_RCV_NFC")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Ad4RcvWlCnnt).HasColumnName("AD4_RCV_WL_CNNT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
