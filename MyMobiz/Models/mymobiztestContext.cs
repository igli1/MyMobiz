using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyMobiz.Models;
namespace MyMobiz.Models
{
    public partial class mymobiztestContext : DbContext
    {
        public mymobiztestContext()
        {
        }

        public mymobiztestContext(DbContextOptions<mymobiztestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Legs> Legs { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<Quotes> Quotes { get; set; }
        public virtual DbSet<Referers> Referers { get; set; }
        public virtual DbSet<Rides> Rides { get; set; }
        public virtual DbSet<Rideslegs> Rideslegs { get; set; }
        public virtual DbSet<Servicerates> Servicerates { get; set; }
        public virtual DbSet<Services> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=198.38.85.103;database=mymobiztest;uid=igli;pwd=igli123", x => x.ServerVersion("5.6.48-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clients>(entity =>
            {
                entity.ToTable("clients");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Legs>(entity =>
            {
                entity.ToTable("legs");

                entity.HasIndex(e => e.FromPlaceId)
                    .HasName("FromPlaceID");

                entity.HasIndex(e => e.ToPlaceId)
                    .HasName("ToPlaceID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FromPlaceId)
                    .HasColumnName("FromPlaceID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Kms).HasColumnType("int(11)");

                entity.Property(e => e.MinutesDrive).HasColumnType("int(11)");

                entity.Property(e => e.MinutesWithTraffic).HasColumnType("int(11)");

                entity.Property(e => e.ToPlaceId)
                    .HasColumnName("ToPlaceID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.FromPlace)
                    .WithMany(p => p.LegsFromPlace)
                    .HasForeignKey(d => d.FromPlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("legs_ibfk_1");

                entity.HasOne(d => d.ToPlace)
                    .WithMany(p => p.LegsToPlace)
                    .HasForeignKey(d => d.ToPlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("legs_ibfk_2");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.ToTable("orders");

                entity.HasIndex(e => e.ClientId)
                    .HasName("ClientID");

                entity.HasIndex(e => e.QuoteId)
                    .HasName("QuoteID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasColumnName("ClientID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.QuoteId)
                    .IsRequired()
                    .HasColumnName("QuoteID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orders_ibfk_2");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orders_ibfk_1");
            });

            modelBuilder.Entity<Places>(entity =>
            {
                entity.ToTable("places");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lat).HasColumnType("decimal(10,0)");

                entity.Property(e => e.Lng).HasColumnType("decimal(10,0)");
            });

            modelBuilder.Entity<Quotes>(entity =>
            {
                entity.ToTable("quotes");

                entity.HasIndex(e => e.RefererId)
                    .HasName("RefererID");

                entity.HasIndex(e => e.RideId)
                    .HasName("RideID");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("ServiceID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RefererId)
                    .HasColumnName("RefererID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RideId)
                    .IsRequired()
                    .HasColumnName("RideID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.VerNum).HasColumnType("int(11)");

                entity.HasOne(d => d.Referer)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.RefererId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quotes_ibfk_3");

                entity.HasOne(d => d.Ride)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.RideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quotes_ibfk_1");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quotes_ibfk_2");
            });

            modelBuilder.Entity<Referers>(entity =>
            {
                entity.ToTable("referers");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("FK_Referers_Services_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Referer)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Tsd)
                    .HasColumnName("TSD")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tsi)
                    .HasColumnName("TSI")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Tsu)
                    .HasColumnName("TSU")
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Referers)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Referers_Services");
            });

            modelBuilder.Entity<Rides>(entity =>
            {
                entity.ToTable("rides");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ClientRanking)
                    .HasColumnType("varchar(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DriverRanking)
                    .HasColumnType("varchar(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Rideslegs>(entity =>
            {
                entity.ToTable("rideslegs");

                entity.HasIndex(e => e.LegId)
                    .HasName("LegID");

                entity.HasIndex(e => e.RideId)
                    .HasName("RideID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DateTimeArrival).HasColumnType("datetime");

                entity.Property(e => e.DateTimeArrivalTh).HasColumnType("datetime");

                entity.Property(e => e.DateTimePickup).HasColumnType("datetime");

                entity.Property(e => e.DateTimePickupTh).HasColumnType("datetime");

                entity.Property(e => e.LegId)
                    .HasColumnName("LegID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RideId)
                    .IsRequired()
                    .HasColumnName("RideID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Seqnr)
                    .HasColumnName("seqnr")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Leg)
                    .WithMany(p => p.Rideslegs)
                    .HasForeignKey(d => d.LegId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("rideslegs_ibfk_2");

                entity.HasOne(d => d.Ride)
                    .WithMany(p => p.Rideslegs)
                    .HasForeignKey(d => d.RideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("rideslegs_ibfk_1");
            });

            modelBuilder.Entity<Servicerates>(entity =>
            {
                entity.HasKey(e => e.VerNum)
                    .HasName("PRIMARY");

                entity.ToTable("servicerates");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("ServiceID");

                entity.Property(e => e.VerNum).HasColumnType("int(11)");

                entity.Property(e => e.AppDate)
                    .HasColumnName("appDate")
                    .HasColumnType("date");

                entity.Property(e => e.DefDate)
                    .HasColumnName("defDate")
                    .HasColumnType("date");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Servicerates)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("servicerates_ibfk_1");
            });

            modelBuilder.Entity<Services>(entity =>
            {
                entity.ToTable("services");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ApiKey)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServiceName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Tsd)
                    .HasColumnName("TSD")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tsi)
                    .HasColumnName("TSI")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Tsu)
                    .HasColumnName("TSU")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
