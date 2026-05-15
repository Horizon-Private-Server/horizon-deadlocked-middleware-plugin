using Horizon.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Middleware.Plugin.Deadlocked.DTO
{
    public partial class DimForgeCgmMaps
    {
        public string MapFilename { get; set; }
        public string Name { get; set; }
        public string? SharedRankCode { get; set; }
        public string? Metadata { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public partial class ForgeCgmMapAccountStat
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string MapFilename { get; set; }

        public int Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int GamesPlayed { get; set; }
        public long TimePlayedMs { get; set; }

        public long TrackedStat1 { get; set; }
        public long TrackedStat2 { get; set; }
        public long TrackedStat3 { get; set; }
        public long TrackedStat4 { get; set; }
        public long TrackedStat5 { get; set; }
        public long TrackedStat6 { get; set; }
        public long TrackedStat7 { get; set; }
        public long TrackedStat8 { get; set; }

        public DateTime? ModifiedDt { get; set; }
    }

    public partial class ForgeCgmMapAggregatedAccountStat
    {
        public int AccountId { get; set; }

        public int Wins { get; set; }
        public int Losses { get; set; }
        public int GamesPlayed { get; set; }
        public long TimePlayedMs { get; set; }
        public int Rank { get; set; }
        public long Ranking { get; set; }

        public long TrackedStat1 { get; set; }
        public long TrackedStat2 { get; set; }
        public long TrackedStat3 { get; set; }
        public long TrackedStat4 { get; set; }
        public long TrackedStat5 { get; set; }
        public long TrackedStat6 { get; set; }
        public long TrackedStat7 { get; set; }
        public long TrackedStat8 { get; set; }
    }
}
