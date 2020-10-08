using Microsoft.EntityFrameworkCore;

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
        public virtual DbSet<Langs> Langs { get; set; }
        public virtual DbSet<Legs> Legs { get; set; }
        public virtual DbSet<Lexicons> Lexicons { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<Quotes> Quotes { get; set; }
        public virtual DbSet<Ratecategories> Ratecategories { get; set; }
        public virtual DbSet<Ratedetails> Ratedetails { get; set; }
        public virtual DbSet<Rategroupings> Rategroupings { get; set; }
        public virtual DbSet<Ratetargets> Ratetargets { get; set; }
        public virtual DbSet<Ridelegs> Ridelegs { get; set; }
        public virtual DbSet<Rides> Rides { get; set; }
        public virtual DbSet<Servicelangs> Servicelangs { get; set; }
        public virtual DbSet<Servicerates> Servicerates { get; set; }
        public virtual DbSet<Services> Services { get; set; }
        public virtual DbSet<Webreferers> Webreferers { get; set; }

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

            modelBuilder.Entity<Langs>(entity =>
            {
                entity.HasKey(e => e.Lang)
                    .HasName("PRIMARY");

                entity.ToTable("langs");

                entity.Property(e => e.Lang)
                    .HasColumnName("lang")
                    .HasColumnType("char(2)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Word)
                    .IsRequired()
                    .HasColumnName("word")
                    .HasColumnType("varchar(50)")
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

            modelBuilder.Entity<Lexicons>(entity =>
            {
                entity.ToTable("lexicons");

                entity.HasIndex(e => e.Lang)
                    .HasName("lexicons_ibfk_2");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("lexicons_ibfk_1");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lang)
                    .IsRequired()
                    .HasColumnName("lang")
                    .HasColumnType("char(2)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lexo)
                    .IsRequired()
                    .HasColumnName("lexo")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Word)
                    .IsRequired()
                    .HasColumnName("word")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.LangNavigation)
                    .WithMany(p => p.Lexicons)
                    .HasForeignKey(d => d.Lang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lexicons_ibfk_2");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Lexicons)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lexicons_ibfk_1");
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

            modelBuilder.Entity<Quotes>(entity =>
            {
                entity.ToTable("quotes");

                entity.HasIndex(e => e.RefererId)
                    .HasName("RefererID");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("ServiceID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("char(11)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.RefererId)
                    .HasColumnName("RefererID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(11)")
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

                entity.Property(e => e.VerNum).HasColumnType("int(11)");

                entity.HasOne(d => d.Referer)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.RefererId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quotes_ibfk_3");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quotes_ibfk_2");
            });

            modelBuilder.Entity<Ratecategories>(entity =>
            {
                entity.ToTable("ratecategories");

                entity.HasIndex(e => e.RateGrouping)
                    .HasName("ratecategories_ibfk_1");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("ratecategories_idfk_2");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryConditions)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lexo)
                    .IsRequired()
                    .HasColumnName("lexo")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RateGrouping)
                    .HasColumnType("varchar(30)")
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
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.RateGroupingNavigation)
                    .WithMany(p => p.Ratecategories)
                    .HasForeignKey(d => d.RateGrouping)
                    .HasConstraintName("ratecategories_ibfk_1");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Ratecategories)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratecategories_idfk_2");
            });

            modelBuilder.Entity<Ratedetails>(entity =>
            {
                entity.ToTable("ratedetails");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("ratesdetails_ibfk_2");

                entity.HasIndex(e => e.Vernum)
                    .HasName("Vernum");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("CategoryID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DetailConditions)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lexo)
                    .HasColumnName("lexo")
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

                entity.Property(e => e.Vernum).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Ratedetails)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratedetails_ibfk_2");

                entity.HasOne(d => d.VernumNavigation)
                    .WithMany(p => p.Ratedetails)
                    .HasForeignKey(d => d.Vernum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratedetails_ibfk_1");
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

            modelBuilder.Entity<Ratetargets>(entity =>
            {
                entity.ToTable("ratetargets");

                entity.HasIndex(e => e.RateDetailId)
                    .HasName("ratetargets_ibfk_1");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RateDetailId)
                    .HasColumnName("RateDetailID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RateFigure).HasColumnType("decimal(10,2)");

                entity.Property(e => e.RateOperator)
                    .IsRequired()
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("'+'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RateTarget)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("'?'")
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

                entity.HasOne(d => d.RateDetail)
                    .WithMany(p => p.Ratetargets)
                    .HasForeignKey(d => d.RateDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratetargets_ibfk_1");
            });

            modelBuilder.Entity<Ridelegs>(entity =>
            {
                entity.ToTable("ridelegs");

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

                entity.HasOne(d => d.Leg)
                    .WithMany(p => p.Ridelegs)
                    .HasForeignKey(d => d.LegId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ridelegs_ibfk_2");

                entity.HasOne(d => d.Ride)
                    .WithMany(p => p.Ridelegs)
                    .HasForeignKey(d => d.RideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ridelegs_ibfk_1");
            });

            modelBuilder.Entity<Rides>(entity =>
            {
                entity.ToTable("rides");

                entity.HasIndex(e => e.QuoteId)
                    .HasName("rides_ibfk_1");

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

                entity.Property(e => e.QuoteId)
                    .HasColumnName("QuoteID")
                    .HasColumnType("char(11)")
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

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.Rides)
                    .HasForeignKey(d => d.QuoteId)
                    .HasConstraintName("rides_ibfk_1");
            });

            modelBuilder.Entity<Servicelangs>(entity =>
            {
                entity.ToTable("servicelangs");

                entity.HasIndex(e => e.Lang)
                    .HasName("servicelangs_ibfk_1");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("servielangs_ibfk_2");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lang)
                    .IsRequired()
                    .HasColumnName("lang")
                    .HasColumnType("char(2)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.LangNavigation)
                    .WithMany(p => p.Servicelangs)
                    .HasForeignKey(d => d.Lang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("servicelangs_ibfk_1");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Servicelangs)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("servielangs_ibfk_2");
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

                entity.Property(e => e.Lexo)
                    .HasColumnName("lexo")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaxPax)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'3'");

                entity.Property(e => e.NQuotes)
                    .HasColumnName("nQuotes")
                    .HasColumnType("int(11)");

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

            modelBuilder.Entity<Webreferers>(entity =>
            {
                entity.ToTable("webreferers");

                entity.HasIndex(e => e.ServiceId)
                    .HasName("WebReferers_ibfk_1");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Referer)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasColumnName("ServiceID")
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Webreferers)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WebReferers_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
