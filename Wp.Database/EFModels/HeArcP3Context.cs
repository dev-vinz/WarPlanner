using Microsoft.EntityFrameworkCore;
using Wp.Database.Settings;

namespace Wp.Database.EFModels;

public partial class HeArcP3Context : DbContext
{
    public HeArcP3Context()
    {
    }

    public HeArcP3Context(DbContextOptions<HeArcP3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Calendar> Calendars { get; set; }

    public virtual DbSet<Clan> Clans { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Guild> Guilds { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerStatistic> PlayerStatistics { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Time> Times { get; set; }

    public virtual DbSet<WarStatistic> WarStatistics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(DBSettings.ToString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasKey(e => e.Guild);

            entity.ToTable("Calendar");

            entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.CalendarId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ChannelId).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.MessageId).HasColumnType("decimal(25, 0)");

            entity.HasOne(d => d.GuildNavigation).WithOne(p => p.Calendar)
                .HasForeignKey<Calendar>(d => d.Guild)
                .HasConstraintName("FK__Calendar__Guild__160F4887");
        });

        modelBuilder.Entity<Clan>(entity =>
        {
            entity.HasKey(e => new { e.Guild, e.Tag }).HasName("PK__Clan__B5E60FD0D2E01C61");

            entity.ToTable("Clan");

            entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Tag)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.Clans)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__Clan__Guild__1332DBDC");
        });

        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => new { e.Guild, e.CategoryId }).HasName("PK__Competit__28338A316397BC18");

            entity.ToTable("Competition");

            entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.CategoryId).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.MainClan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.ResultId).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.SecondClan)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.Competitions)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__Competiti__Guild__1BC821DD");
        });

        modelBuilder.Entity<Guild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Guild__3214EC07C21ADC14");

            entity.ToTable("Guild");

            entity.Property(e => e.Id).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.MinThlevel).HasColumnName("MinTHLevel");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => new { e.Guild, e.Tag }).HasName("PK__Player__B5E60FD0AFCBB9FE");

            entity.ToTable("Player");

            entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Tag)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DiscordId).HasColumnType("decimal(25, 0)");

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.Players)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__Player__Guild__2180FB33");
        });

        modelBuilder.Entity<PlayerStatistic>(entity =>
        {
            entity.HasKey(e => new { e.DiscordId, e.ClanTag, e.WarDateStart, e.AttackOrder }).HasName("PK__PlayerSt__195E8FEB39725BD4");

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

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.PlayerStatistics)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__PlayerSta__Guild__2739D489");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Type }).HasName("PK__Role__8D8F664FB0CB761C");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.Roles)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__Role__Guild__18EBB532");
        });

        modelBuilder.Entity<Time>(entity =>
        {
            entity.HasKey(e => new { e.Guild, e.Action, e.Additional }).HasName("PK__Time__F473DB486641416E");

            entity.ToTable("Time");

            entity.Property(e => e.Guild).HasColumnType("decimal(25, 0)");
            entity.Property(e => e.Additional)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Optional).IsUnicode(false);

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.Times)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__Time__Guild__1EA48E88");
        });

        modelBuilder.Entity<WarStatistic>(entity =>
        {
            entity.HasKey(e => new { e.DateStart, e.ClanTag }).HasName("PK__WarStati__8099B70DDED0D215");

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

            entity.HasOne(d => d.GuildNavigation).WithMany(p => p.WarStatistics)
                .HasForeignKey(d => d.Guild)
                .HasConstraintName("FK__WarStatis__Guild__245D67DE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
