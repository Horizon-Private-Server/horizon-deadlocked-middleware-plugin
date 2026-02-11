using Horizon.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Middleware.Plugin.Deadlocked.DTO
{
    public partial class DimSurvivalMaps
    {
        public string MapFilename { get; set; }
        public string MapName { get; set; }
        public int Sort { get; set; }
    }

    public partial class AccountSurvivalStat
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        public long GamesPlayed { get; set; }
        public long TimePlayedMs { get; set; }
        public long Kills { get; set; }
        public long Deaths { get; set; }
        public long Revives { get; set; }
        public long TimesRevived { get; set; }

        public long WrenchKills { get; set; }
        public long DualViperKills { get; set; }
        public long MagmaCannonKills { get; set; }
        public long ArbiterKills { get; set; }
        public long FusionRifleKills { get; set; }
        public long MineLauncherKills { get; set; }
        public long B6Kills { get; set; }
        public long HoloshieldKills { get; set; }
        public long ScorpionFlailKills { get; set; }

        public DateTime? ModifiedDt { get; set; }
    }

    public partial class AccountSurvivalMapStat
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string MapFilename { get; set; }
        public int Xp { get; set; }
        public int Rank { get; set; }
        public int Prestige { get; set; }
        public decimal PercentCompleted { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public partial class AccountSurvivalMapGambitStat
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string MapFilename { get; set; }
        public string Gambit { get; set; }
        public bool Completed { get; set; }
        public int Round { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public partial class AccountSurvivalMapRun
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerCountAtStart { get; set; }
        public string MapFilename { get; set; }
        public string? Gambit { get; set; }
        public int Round { get; set; }
        public long TimeMs { get; set; }
        public long? Time50Ms { get; set; }
        public int? AccountId0 { get; set; }
        public int? AccountId1 { get; set; }
        public int? AccountId2 { get; set; }
        public int? AccountId3 { get; set; }
        public int? AccountId4 { get; set; }
        public int? AccountId5 { get; set; }
        public int? AccountId6 { get; set; }
        public int? AccountId7 { get; set; }
        public int? AccountId8 { get; set; }
        public int? AccountId9 { get; set; }
        public DateTime CreatedDt { get; set; }
    }

    public partial class SurvivalCompletionLeaderboardRow
    {
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public decimal TotalPercentCompleted { get; set; }
        public long LeaderboardRank { get; set; }
    }

    public partial class SurvivalLeaderboardRow
    {
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public long Value { get; set; }
        public long LeaderboardRank { get; set; }
    }

    public partial class SurvivalMapLeaderboardRow
    {
        public string AccountIds { get; set; }
        public string AccountNames { get; set; }
        public long Value { get; set; }
        public long LeaderboardRank { get; set; }
    }

    public partial class SurvivalRunLeaderboardRow
    {
        public int? AccountId0 { get; set; }
        public int? AccountId1 { get; set; }
        public int? AccountId2 { get; set; }
        public int? AccountId3 { get; set; }
        public int? AccountId4 { get; set; }
        public int? AccountId5 { get; set; }
        public int? AccountId6 { get; set; }
        public int? AccountId7 { get; set; }
        public int? AccountId8 { get; set; }
        public int? AccountId9 { get; set; }
        public string? AccountNames { get; set; }
        public int PlayerCountAtStart { get; set; }
        public string MapFilename { get; set; }
        public string? Gambit { get; set; }
        public long Value { get; set; }
        public long LeaderboardRank { get; set; }
    }

    public partial class SurvivalAccountOverallMapStats
    {
        public long? OverallRanking { get; set; }
        public long? RoundRanking { get; set; }
        public long? Time50Ranking { get; set; }

        public int? BestSoloRound { get; set; }
        public long? BestSoloTime50 { get; set; }
        public int? BestCoopRound { get; set; }
        public long? BestCoopTime50 { get; set; }
    }
}
