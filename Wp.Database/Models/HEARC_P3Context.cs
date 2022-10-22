using Microsoft.EntityFrameworkCore;
using Wp.Database.Settings;

namespace Wp.Database.Models
{
    public partial class HEARC_P3Context : DbContext
    {
        public HEARC_P3Context()
        {
        }

        public HEARC_P3Context(DbContextOptions<HEARC_P3Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Calendar> Calendars { get; set; } = null!;
        public virtual DbSet<Clan> Clans { get; set; } = null!;
        public virtual DbSet<Competition> Competitions { get; set; } = null!;
        public virtual DbSet<Guild> Guilds { get; set; } = null!;
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<PlayerStatistic> PlayerStatistics { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Time> Times { get; set; } = null!;
        public virtual DbSet<WarStatistic> WarStatistics { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DBSettings.ToString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.HasKey(e => new { e.Guild, e.CalendarId })
                    .HasName("PK__Calendar__4C9FE5D5B6BBF550");

                entity.ToTable("Calendar");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.CalendarId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ChannelId).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.MessageId).HasColumnType("decimal(25, 0)");

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.Calendars)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__Calendar__Guild__3B75D760");
            });

            modelBuilder.Entity<Clan>(entity =>
            {
                entity.HasKey(e => new { e.Guild, e.Tag })
                    .HasName("PK__Clan__B5E60FD0AE15DB2E");

                entity.ToTable("Clan");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.Tag)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.Clans)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__Clan__Guild__38996AB5");
            });

            modelBuilder.Entity<Competition>(entity =>
            {
                entity.HasKey(e => new { e.Guild, e.CategoryId })
                    .HasName("PK__Competit__28338A31134F1059");

                entity.ToTable("Competition");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.CategoryId).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.MainClan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResultId).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.SecondClan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.Competitions)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__Competiti__Guild__412EB0B6");
            });

            modelBuilder.Entity<Guild>(entity =>
            {
                entity.ToTable("Guild");

                entity.Property(e => e.Id).HasColumnType("decimal(25, 0)");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => new { e.Guild, e.Tag })
                    .HasName("PK__Player__B5E60FD032418408");

                entity.ToTable("Player");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.Tag)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DiscordId).HasColumnType("decimal(25, 0)");

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__Player__Guild__46E78A0C");
            });

            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(e => new { e.DiscordId, e.ClanTag, e.WarDateStart, e.AttackOrder })
                    .HasName("PK__PlayerSt__195E8FEBD381F0EA");

                entity.ToTable("PlayerStatistic");

                entity.Property(e => e.DiscordId).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.ClanTag)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WarDateStart).HasColumnType("datetime");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.PlayerTag)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.PlayerStatistics)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__PlayerSta__Guild__4CA06362");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type })
                    .HasName("PK__Role__8D8F664F038A18CB");

                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__Role__Guild__3E52440B");
            });

            modelBuilder.Entity<Time>(entity =>
            {
                entity.HasKey(e => new { e.Guild, e.Action, e.Additional })
                    .HasName("PK__Time__F473DB48EE40DD89");

                entity.ToTable("Time");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.Additional)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Optional).IsUnicode(false);

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.Times)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__Time__Guild__440B1D61");
            });

            modelBuilder.Entity<WarStatistic>(entity =>
            {
                entity.HasKey(e => new { e.DateStart, e.ClanTag })
                    .HasName("PK__WarStati__8099B70D7509A06C");

                entity.ToTable("WarStatistic");

                entity.Property(e => e.DateStart).HasColumnType("datetime");

                entity.Property(e => e.ClanTag)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AttackAvgDuration).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.AttackPercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.CompetitionCategory).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.DefenseAvgDuration).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.DefensePercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

                entity.Property(e => e.OpponentName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.GuildNavigation)
                    .WithMany(p => p.WarStatistics)
                    .HasForeignKey(d => d.Guild)
                    .HasConstraintName("FK__WarStatis__Guild__49C3F6B7");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
