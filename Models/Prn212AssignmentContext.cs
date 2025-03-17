using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project212.Models;

public partial class Prn212AssignmentContext : DbContext
{
    public Prn212AssignmentContext()
    {
    }

    public Prn212AssignmentContext(DbContextOptions<Prn212AssignmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Citizen> Citizens { get; set; }

    public virtual DbSet<InspectionStation> InspectionStations { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Notice> Notices { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Standard> Standards { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =localhost; database = PRN212_Assignment;uid=sa;pwd=1234;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Citizen>(entity =>
        {
            entity.ToTable("Citizen");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Mail)
                .HasMaxLength(50)
                .HasColumnName("mail");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");

            entity.HasOne(d => d.Acc).WithMany(p => p.Citizens)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citizen_Account");
        });

        modelBuilder.Entity<InspectionStation>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Mail)
                .HasMaxLength(50)
                .HasColumnName("mail");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Acc).WithMany(p => p.Logs)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Account");
        });

        modelBuilder.Entity<Notice>(entity =>
        {
            entity.ToTable("Notice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.Detail)
                .HasMaxLength(250)
                .HasColumnName("detail");
            entity.Property(e => e.IsRead).HasColumnName("isRead");
            entity.Property(e => e.SentDate)
                .HasColumnType("datetime")
                .HasColumnName("sentDate");

            entity.HasOne(d => d.Acc).WithMany(p => p.Notices)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notice_Account");
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.ToTable("Record");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Co)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("CO");
            entity.Property(e => e.Hc)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("HC");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Nox)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("NOx");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.StandardId).HasColumnName("standardID");
            entity.Property(e => e.TimeId).HasColumnName("timeID");
            entity.Property(e => e.VehicleId).HasColumnName("vehicleID");

            entity.HasOne(d => d.Standard).WithMany(p => p.Records)
                .HasForeignKey(d => d.StandardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Record_Standards");

            entity.HasOne(d => d.Time).WithMany(p => p.Records)
                .HasForeignKey(d => d.TimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Record_Timetable");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Records)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Record_Vehicle");
        });

        modelBuilder.Entity<Standard>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Co).HasColumnName("CO");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Hc).HasColumnName("HC");
            entity.Property(e => e.Nox).HasColumnName("NOx");
            entity.Property(e => e.VehicleType).HasColumnName("vehicleType");
        });

        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.ToTable("Timetable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.InspectTime)
                .HasColumnType("datetime")
                .HasColumnName("inspectTime");
            entity.Property(e => e.InspectionId).HasColumnName("inspectionID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("status");

            entity.HasOne(d => d.Acc).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Account");

            entity.HasOne(d => d.Inspection).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.InspectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_InspectionStations");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicle");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Brand)
                .HasMaxLength(50)
                .HasColumnName("brand");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Chassis)
                .HasMaxLength(12)
                .IsFixedLength()
                .HasColumnName("chassis");
            entity.Property(e => e.CitizenId).HasColumnName("citizenID");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.Dom).HasColumnName("dom");
            entity.Property(e => e.Engine)
                .HasMaxLength(12)
                .IsFixedLength()
                .HasColumnName("engine");
            entity.Property(e => e.Model)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("model");
            entity.Property(e => e.Plate)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("plate");

            entity.HasOne(d => d.Citizen).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CitizenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Citizen");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
