using Horizon.Database.Entities;
using Horizon.Middleware.Plugin.Deadlocked.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Middleware.Plugin.Deadlocked.Entities
{
    public partial class CustomDbContext : DbContext
    {
        public CustomDbContext()
        {
        }

        public CustomDbContext(DbContextOptions<CustomDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DimSurvivalMaps> DimSurvivalMaps { get; set; }
        public virtual DbSet<AccountSurvivalStat> AccountSurvivalStat { get; set; }
        public virtual DbSet<AccountSurvivalMapStat> AccountSurvivalMapStat { get; set; }
        public virtual DbSet<AccountSurvivalMapGambitStat> AccountSurvivalMapGambitStat { get; set; }
        public virtual DbSet<AccountSurvivalMapRun> AccountSurvivalMapRun { get; set; }

        public virtual DbSet<SurvivalAccountOverallMapStats> SurvivalAccountLeaderboardRanking { get; set; }
        public virtual DbSet<SurvivalCompletionLeaderboardRow> SurvivalCompletionLeaderboardRow { get; set; }
        public virtual DbSet<SurvivalLeaderboardRow> SurvivalLeaderboardRow { get; set; }
        public virtual DbSet<SurvivalMapLeaderboardRow> SurvivalMapLeaderboardRow { get; set; }
        public virtual DbSet<SurvivalRunLeaderboardRow> SurvivalRunLeaderboardRow { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DimSurvivalMaps>(entity =>
            {
                entity.ToTable("dim_survival_maps", "KEYS").HasNoKey();

                entity.Property(e => e.MapFilename).HasColumnName("map_filename");
                entity.Property(e => e.MapName).HasColumnName("map_name");
                entity.Property(e => e.Sort).HasColumnName("sort");
            });

            modelBuilder.Entity<AccountSurvivalStat>(entity =>
            {
                entity.ToTable("account_survival_stat", "STATS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.GamesPlayed).HasColumnName("games_played");
                entity.Property(e => e.TimePlayedMs).HasColumnName("time_played_ms");
                entity.Property(e => e.Kills).HasColumnName("kills");
                entity.Property(e => e.Deaths).HasColumnName("deaths");
                entity.Property(e => e.Revives).HasColumnName("revives");
                entity.Property(e => e.TimesRevived).HasColumnName("times_revived");
                entity.Property(e => e.WrenchKills).HasColumnName("wrench_kills");
                entity.Property(e => e.DualViperKills).HasColumnName("dual_vipers_kills");
                entity.Property(e => e.MagmaCannonKills).HasColumnName("magma_cannon_kills");
                entity.Property(e => e.ArbiterKills).HasColumnName("arbiter_kills");
                entity.Property(e => e.FusionRifleKills).HasColumnName("fusion_rifle_kills");
                entity.Property(e => e.MineLauncherKills).HasColumnName("mine_launcher_kills");
                entity.Property(e => e.B6Kills).HasColumnName("b6_kills");
                entity.Property(e => e.HoloshieldKills).HasColumnName("holoshield_kills");
                entity.Property(e => e.ScorpionFlailKills).HasColumnName("scorpion_flail_kills");

                entity.Property(e => e.ModifiedDt).HasColumnName("modified_dt");
            });

            modelBuilder.Entity<AccountSurvivalMapStat>(entity =>
            {
                entity.ToTable("account_survival_map_stat", "STATS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.MapFilename).HasColumnName("map_filename");

                entity.Property(e => e.Xp).HasColumnName("xp");
                entity.Property(e => e.Rank).HasColumnName("rank");
                entity.Property(e => e.Prestige).HasColumnName("prestige");
                entity.Property(e => e.PercentCompleted).HasColumnName("percent_completed").HasColumnType("DECIMAL(6,5)");

                entity.Property(e => e.ModifiedDt).HasColumnName("modified_dt");
            });

            modelBuilder.Entity<AccountSurvivalMapGambitStat>(entity =>
            {
                entity.ToTable("account_survival_gambit_stat", "STATS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.MapFilename).HasColumnName("map_filename");
                entity.Property(e => e.Gambit).HasColumnName("gambit");

                entity.Property(e => e.Completed).HasColumnName("completed");
                entity.Property(e => e.Round).HasColumnName("round");

                entity.Property(e => e.ModifiedDt).HasColumnName("modified_dt");
            });

            modelBuilder.Entity<AccountSurvivalMapRun>(entity =>
            {
                entity.ToTable("account_survival_map_run", "STATS");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.GameId).HasColumnName("game_id");
                entity.Property(e => e.PlayerCountAtStart).HasColumnName("player_count_at_start");
                entity.Property(e => e.MapFilename).HasColumnName("map_filename");
                entity.Property(e => e.Gambit).HasColumnName("gambit");

                entity.Property(e => e.Round).HasColumnName("round");
                entity.Property(e => e.TimeMs).HasColumnName("time_ms");
                entity.Property(e => e.Time50Ms).HasColumnName("time_50_ms");
                entity.Property(e => e.AccountId0).HasColumnName("account_id_0");
                entity.Property(e => e.AccountId1).HasColumnName("account_id_1");
                entity.Property(e => e.AccountId2).HasColumnName("account_id_2");
                entity.Property(e => e.AccountId3).HasColumnName("account_id_3");
                entity.Property(e => e.AccountId4).HasColumnName("account_id_4");
                entity.Property(e => e.AccountId5).HasColumnName("account_id_5");
                entity.Property(e => e.AccountId6).HasColumnName("account_id_6");
                entity.Property(e => e.AccountId7).HasColumnName("account_id_7");
                entity.Property(e => e.AccountId8).HasColumnName("account_id_8");
                entity.Property(e => e.AccountId9).HasColumnName("account_id_9");

                entity.Property(e => e.CreatedDt)
                    .HasColumnName("created_dt")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<SurvivalAccountOverallMapStats>(entity =>
            {
                entity.HasNoKey().ToView(null);

                entity.Property(e => e.OverallRanking).HasColumnName("overall_rank");
                entity.Property(e => e.RoundRanking).HasColumnName("round_rank");
                entity.Property(e => e.Time50Ranking).HasColumnName("time_50_rank");
                entity.Property(e => e.BestSoloRound).HasColumnName("solo_round");
                entity.Property(e => e.BestSoloTime50).HasColumnName("solo_time_50");
                entity.Property(e => e.BestCoopRound).HasColumnName("coop_round");
                entity.Property(e => e.BestCoopTime50).HasColumnName("coop_time_50");
            });

            modelBuilder.Entity<SurvivalCompletionLeaderboardRow>(entity =>
            {
                entity.HasNoKey().ToView(null);

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.AccountName).HasColumnName("account_name");
                entity.Property(e => e.TotalPercentCompleted).HasColumnName("total_percent_completed").HasColumnType("DECIMAL(6,5)");
                entity.Property(e => e.LeaderboardRank).HasColumnName("leaderboard_rank");
            });

            modelBuilder.Entity<SurvivalLeaderboardRow>(entity =>
            {
                entity.HasNoKey().ToView(null);

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.AccountName).HasColumnName("account_name");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.LeaderboardRank).HasColumnName("leaderboard_rank");
            });

            modelBuilder.Entity<SurvivalMapLeaderboardRow>(entity =>
            {
                entity.HasNoKey().ToView(null);

                entity.Property(e => e.AccountIds).HasColumnName("account_ids");
                entity.Property(e => e.AccountNames).HasColumnName("account_names");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.LeaderboardRank).HasColumnName("leaderboard_rank");
            });

            modelBuilder.Entity<SurvivalRunLeaderboardRow>(entity =>
            {
                entity.HasNoKey().ToView(null);

                entity.Property(e => e.AccountId0).HasColumnName("account_id_0");
                entity.Property(e => e.AccountId1).HasColumnName("account_id_1");
                entity.Property(e => e.AccountId2).HasColumnName("account_id_2");
                entity.Property(e => e.AccountId3).HasColumnName("account_id_3");
                entity.Property(e => e.AccountId4).HasColumnName("account_id_4");
                entity.Property(e => e.AccountId5).HasColumnName("account_id_5");
                entity.Property(e => e.AccountId6).HasColumnName("account_id_6");
                entity.Property(e => e.AccountId7).HasColumnName("account_id_7");
                entity.Property(e => e.AccountId8).HasColumnName("account_id_8");
                entity.Property(e => e.AccountId9).HasColumnName("account_id_9");
                entity.Property(e => e.AccountNames).HasColumnName("account_names");
                entity.Property(e => e.PlayerCountAtStart).HasColumnName("player_count_at_start");
                entity.Property(e => e.MapFilename).HasColumnName("map_filename");
                entity.Property(e => e.Gambit).HasColumnName("gambit");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.LeaderboardRank).HasColumnName("leaderboard_rank");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
