using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace MobizAdmin.Data
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

        public virtual DbSet<Ratecategories> Ratecategories { get; set; }
        public virtual DbSet<Rategroupings> Rategroupings { get; set; }
        public virtual DbSet<Ratesdetails> Ratesdetails { get; set; }
        public virtual DbSet<Ratetargets> Ratetargets { get; set; }
        public virtual DbSet<Referers> Referers { get; set; }
        public virtual DbSet<Servicerates> Servicerates { get; set; }
        public virtual DbSet<Services> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<Ratecategories>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PRIMARY");

                entity.ToTable("ratecategories");

                entity.HasIndex(e => e.RateGrouping)
                    .HasName("ratecategories_ibfk_1");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("CategoryID")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CategoryConditions)
                    .HasColumnType("varchar(10000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Naming)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RateGrouping)
                    .HasColumnType("varchar(30)")
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

                entity.HasOne(d => d.RateGroupingNavigation)
                    .WithMany(p => p.Ratecategories)
                    .HasForeignKey(d => d.RateGrouping)
                    .HasConstraintName("ratecategories_ibfk_1");
            });

            modelBuilder.Entity<Rategroupings>(entity =>
            {
                entity.HasKey(e => e.RateGrouping)
                    .HasName("PRIMARY");

                entity.ToTable("rategroupings");

                entity.Property(e => e.RateGrouping)
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Ratesdetails>(entity =>
            {
                entity.ToTable("ratesdetails");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("ratesdetails_ibfk_2");

                entity.HasIndex(e => e.RateTarget)
                    .HasName("ratesdetails_ibfk_3");

                entity.HasIndex(e => e.Vernum)
                    .HasName("Vernum");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasColumnName("CategoryID")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RateFigure).HasColumnType("decimal(10,2)");

                entity.Property(e => e.RateOperator)
                    .IsRequired()
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RateTarget)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
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

                entity.Property(e => e.Vernum).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Ratesdetails)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratesdetails_ibfk_2");

                entity.HasOne(d => d.RateTargetNavigation)
                    .WithMany(p => p.Ratesdetails)
                    .HasForeignKey(d => d.RateTarget)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratesdetails_ibfk_3");

                entity.HasOne(d => d.VernumNavigation)
                    .WithMany(p => p.Ratesdetails)
                    .HasForeignKey(d => d.Vernum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratesdetails_ibfk_1");
            });

            modelBuilder.Entity<Ratetargets>(entity =>
            {
                entity.HasKey(e => e.RateTarget)
                    .HasName("PRIMARY");

                entity.ToTable("ratetargets");

                entity.Property(e => e.RateTarget)
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
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

                entity.Property(e => e.EndDate)
                    .HasColumnName("endDate")
                    .HasColumnType("date");

                entity.Property(e => e.EurKm).HasColumnType("decimal(10,2)");

                entity.Property(e => e.EurMinDrive).HasColumnType("decimal(10,2)");

                entity.Property(e => e.EurMinWait).HasColumnType("decimal(10,2)");

                entity.Property(e => e.EurMinimum).HasColumnType("decimal(10,2)");

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
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

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
