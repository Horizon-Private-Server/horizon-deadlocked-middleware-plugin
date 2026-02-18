using Horizon.Database.Entities;
using Horizon.Database.Services;
using Horizon.Middleware.Plugin.Deadlocked.DTO;
using Horizon.Middleware.Plugin.Deadlocked.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Middleware.Plugin.Deadlocked.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SurvivalController : ControllerBase
    {
        private Ratchet_DeadlockedContext db;
        private CustomDbContext customDb;
        private IAuthService authService;
        public SurvivalController(Ratchet_DeadlockedContext _db, CustomDbContext _customDb, IAuthService _authService)
        {
            db = _db;
            customDb = _customDb;
            authService = _authService;
        }

        [Authorize]
        [HttpGet, Route("getMaps")]
        public async Task<dynamic> getMaps()
        {
            return await (from m in customDb.DimSurvivalMaps
                          orderby m.Sort ascending, m.MapName ascending
                          select new SurvivalMapDTO()
                          {
                              Name = m.MapName,
                              Filename = m.MapFilename
                          }).ToListAsync();
        }

        [Authorize]
        [HttpGet, Route("getIsMap/{mapFilename}")]
        public async Task<dynamic> getIsMap(string mapFilename)
        {
            return await (from m in customDb.DimSurvivalMaps
                          where m.MapFilename == mapFilename
                          select new { m.MapFilename }).AnyAsync();
        }

        [Authorize]
        [HttpGet, Route("getAccountStats")]
        public async Task<dynamic> getAccountStats(int AccountId)
        {
            if (AccountId == 0)
                return BadRequest("Missing AccountId");

            var stat = await (from s in customDb.AccountSurvivalStat
                              where s.AccountId == AccountId
                              select new SurvivalAccountStatDTO()
                              {
                                  AccountId = s.AccountId,
                                  GamesPlayed = s.GamesPlayed,
                                  TimePlayedMs = s.TimePlayedMs,
                                  Kills = s.Kills,
                                  Deaths = s.Deaths,
                                  Revives = s.Revives,
                                  TimesRevived = s.TimesRevived,
                                  WrenchKills = s.WrenchKills,
                                  DualViperKills = s.DualViperKills,
                                  MagmaCannonKills = s.MagmaCannonKills,
                                  ArbiterKills = s.ArbiterKills,
                                  FusionRifleKills = s.FusionRifleKills,
                                  MineLauncherKills = s.MineLauncherKills,
                                  B6Kills = s.B6Kills,
                                  HoloshieldKills = s.HoloshieldKills,
                                  ScorpionFlailKills = s.ScorpionFlailKills
                              }).FirstOrDefaultAsync()
                              ?? new SurvivalAccountStatDTO() { AccountId = AccountId };

            var completionStat = (await customDb.SurvivalCompletionLeaderboardRow.FromSqlRaw("SELECT * FROM [dbo].[GetSurvivalCompletionLeaderboard]() WHERE account_id = @p0", AccountId)
                .ToListAsync())
                .FirstOrDefault();

            if (completionStat != null)
            {
                stat.TotalPercentCompleted = completionStat.TotalPercentCompleted;
                stat.Ranking = completionStat.LeaderboardRank;
            }

            return stat;
        }

        [Authorize("database")]
        [HttpPost, Route("updateAccountStats")]
        public async Task<dynamic> updateAccountStats([FromBody] SurvivalAccountStatDTO request)
        {
            if (request.AccountId == 0)
                return BadRequest("Missing AccountId");

            var stats = await customDb.AccountSurvivalStat.FirstOrDefaultAsync(s => s.AccountId == request.AccountId);
            if (stats == null)
            {
                stats = new AccountSurvivalStat()
                {
                    AccountId = request.AccountId,
                    GamesPlayed = request.GamesPlayed,
                    TimePlayedMs = request.TimePlayedMs,
                    Kills = request.Kills,
                    Deaths = request.Deaths,
                    Revives = request.Revives,
                    TimesRevived = request.TimesRevived,
                    WrenchKills = request.WrenchKills,
                    DualViperKills = request.DualViperKills,
                    MagmaCannonKills = request.MagmaCannonKills,
                    ArbiterKills = request.ArbiterKills,
                    FusionRifleKills = request.FusionRifleKills,
                    MineLauncherKills = request.MineLauncherKills,
                    B6Kills = request.B6Kills,
                    HoloshieldKills = request.HoloshieldKills,
                    ScorpionFlailKills = request.ScorpionFlailKills,
                    ModifiedDt = DateTime.UtcNow
                };

                customDb.AccountSurvivalStat.Add(stats);
            }
            else
            {
                stats.GamesPlayed = request.GamesPlayed;
                stats.TimePlayedMs = request.TimePlayedMs;
                stats.Kills = request.Kills;
                stats.Deaths = request.Deaths;
                stats.Revives = request.Revives;
                stats.TimesRevived = request.TimesRevived;
                stats.WrenchKills = request.WrenchKills;
                stats.DualViperKills = request.DualViperKills;
                stats.MagmaCannonKills = request.MagmaCannonKills;
                stats.ArbiterKills = request.ArbiterKills;
                stats.FusionRifleKills = request.FusionRifleKills;
                stats.MineLauncherKills = request.MineLauncherKills;
                stats.B6Kills = request.B6Kills;
                stats.HoloshieldKills = request.HoloshieldKills;
                stats.ScorpionFlailKills = request.ScorpionFlailKills;
                stats.ModifiedDt = DateTime.UtcNow;

                customDb.AccountSurvivalStat.Update(stats);
            }

            await customDb.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet, Route("getAccountMapStats")]
        public async Task<dynamic> getAccountMapStats(int AccountId, string MapFilename)
        {
            if (AccountId == 0)
                return BadRequest("Missing AccountId");

            if (string.IsNullOrEmpty(MapFilename))
                return BadRequest("Missing MapFilename");

            var stat = await (from s in customDb.AccountSurvivalMapStat
                              where s.AccountId == AccountId && s.MapFilename == MapFilename
                              select new SurvivalAccountMapStatDTO()
                              {
                                  AccountId = s.AccountId,
                                  MapFilename = s.MapFilename,
                                  Xp = s.Xp,
                                  Rank = s.Rank,
                                  Prestige = s.Prestige,
                                  PercentCompleted = s.PercentCompleted,
                              }).FirstOrDefaultAsync()
                              ?? new SurvivalAccountMapStatDTO() { AccountId = AccountId, MapFilename = MapFilename };

            var gambits = await (from g in customDb.AccountSurvivalMapGambitStat
                                 where g.AccountId == AccountId && g.MapFilename == MapFilename
                                 select new SurvivalAccountMapGambitStatDTO()
                                 {
                                     Gambit = g.Gambit,
                                     BestRound = g.Round,
                                     Completed = g.Completed
                                 }).ToListAsync();

            var accountMapStats = (await customDb.SurvivalAccountLeaderboardRanking.FromSqlRaw("EXEC [dbo].[GetSurvivalAccountMapStats] @p0, @p1", AccountId, MapFilename).ToListAsync()).FirstOrDefault();
            stat.SoloRoundRanking = accountMapStats?.RoundRanking;
            stat.Solo50Ranking = accountMapStats?.Time50Ranking;
            stat.OverallRanking = accountMapStats?.OverallRanking;
            stat.SoloRound = accountMapStats?.BestSoloRound;
            stat.Solo50 = accountMapStats?.BestSoloTime50;
            stat.CoopRound = accountMapStats?.BestCoopRound;
            stat.Coop50 = accountMapStats?.BestCoopTime50;

            stat.Gambits = gambits;
            return stat;
        }

        [Authorize("database")]
        [HttpPost, Route("updateAccountMapStats")]
        public async Task<dynamic> updateAccountMapStats([FromBody] SurvivalAccountMapStatDTO request)
        {
            if (request.AccountId == 0)
                return BadRequest("Missing AccountId");

            if (string.IsNullOrEmpty(request.MapFilename))
                return BadRequest("Missing MapFilename");

            var stats = await customDb.AccountSurvivalMapStat.FirstOrDefaultAsync(s => s.AccountId == request.AccountId && s.MapFilename == request.MapFilename);
            if (stats == null)
            {
                stats = new AccountSurvivalMapStat()
                {
                    AccountId = request.AccountId,
                    MapFilename = request.MapFilename,
                    Xp = request.Xp,
                    Rank = request.Rank,
                    Prestige = request.Prestige,
                    PercentCompleted = Math.Clamp(request.PercentCompleted, 0, 1),
                    ModifiedDt = DateTime.UtcNow
                };

                customDb.AccountSurvivalMapStat.Add(stats);
            }
            else
            {
                stats.Xp = request.Xp;
                stats.Rank = request.Rank;
                stats.Prestige = request.Prestige;
                stats.PercentCompleted = request.PercentCompleted;
                stats.ModifiedDt = DateTime.UtcNow;

                customDb.AccountSurvivalMapStat.Update(stats);
            }

            await customDb.SaveChangesAsync();
            return Ok();
        }

        [Authorize("database")]
        [HttpPost, Route("updateAccountMapGambitStats")]
        public async Task<dynamic> updateAccountMapGambitStats([FromBody] SurvivalAccountMapGambitStatPostDTO statData)
        {
            if (statData.AccountId == 0)
                return BadRequest("Missing AccountId");

            if (string.IsNullOrEmpty(statData.MapFilename))
                return BadRequest("Missing MapFilename");

            if (string.IsNullOrEmpty(statData.Gambit))
                return BadRequest("Missing gambit");

            var existingStat = await customDb.AccountSurvivalMapGambitStat
                .FirstOrDefaultAsync(s => s.AccountId == statData.AccountId &&
                                         s.MapFilename == statData.MapFilename &&
                                         s.Gambit == statData.Gambit);

            if (existingStat == null)
            {
                var newStat = new AccountSurvivalMapGambitStat
                {
                    AccountId = statData.AccountId,
                    MapFilename = statData.MapFilename,
                    Gambit = statData.Gambit,
                    Completed = statData.Completed ?? false,
                    Round = statData.BestRound ?? 0,
                    ModifiedDt = DateTime.UtcNow
                };
                customDb.AccountSurvivalMapGambitStat.Add(newStat);
            }
            else
            {
                if (statData.BestRound.HasValue && statData.BestRound > existingStat.Round)
                {
                    existingStat.Round = statData.BestRound.Value;
                }

                if (statData.Completed.HasValue)
                {
                    existingStat.Completed = statData.Completed.Value;
                }

                existingStat.ModifiedDt = DateTime.UtcNow;
            }

            // 4. Persist changes to the database
            await customDb.SaveChangesAsync();
            return Ok();
        }

        [Authorize("database")]
        [HttpPost, Route("createMapRun")]
        public async Task<dynamic> createMapRun([FromBody] SurvivalRunDTO request)
        {
            if (string.IsNullOrEmpty(request.MapFilename))
                return BadRequest("Missing MapFilename");

            var game = db.Game.OrderByDescending(x => x.Id).FirstOrDefault(x => x.GameId == request.GameId);
            var accountIds = Enumerable.Range(0, 10).Select(x => request.AccountIds.ElementAtOrDefault(x) > 0 ? request.AccountIds.ElementAt(x) : (int?)null).ToArray();

            var newRun = new AccountSurvivalMapRun
            {
                GameId = game?.Id ?? 0,
                PlayerCountAtStart = request.PlayerCountAtStart,
                MapFilename = request.MapFilename,
                Gambit = request.Gambit,
                Round = request.RoundsCompleted,
                TimeMs = request.TimeMs,
                Time50Ms = request.Time50Ms,
                AccountId0 = accountIds.ElementAtOrDefault(0),
                AccountId1 = accountIds.ElementAtOrDefault(1),
                AccountId2 = accountIds.ElementAtOrDefault(2),
                AccountId3 = accountIds.ElementAtOrDefault(3),
                AccountId4 = accountIds.ElementAtOrDefault(4),
                AccountId5 = accountIds.ElementAtOrDefault(5),
                AccountId6 = accountIds.ElementAtOrDefault(6),
                AccountId7 = accountIds.ElementAtOrDefault(7),
                AccountId8 = accountIds.ElementAtOrDefault(8),
                AccountId9 = accountIds.ElementAtOrDefault(9),
                CreatedDt = DateTime.UtcNow
            };
            customDb.AccountSurvivalMapRun.Add(newRun);

            await customDb.SaveChangesAsync();
            return newRun;
        }

        [Authorize("database")]
        [HttpPost, Route("updateMapRun/{runId}")]
        public async Task<dynamic> updateMapRun(int runId, [FromBody] SurvivalRunDTO request)
        {
            if (string.IsNullOrEmpty(request.MapFilename))
                return BadRequest("Missing MapFilename");

            var run = await customDb.AccountSurvivalMapRun.FirstOrDefaultAsync(s => s.Id == runId);
            if (run == null) return NotFound();

            var accountIds = Enumerable.Range(0, 10).Select(x => request.AccountIds.ElementAtOrDefault(x) > 0 ? request.AccountIds.ElementAt(x) : (int?)null).ToArray();

            run.GameId = request.GameId;
            run.PlayerCountAtStart = request.PlayerCountAtStart;
            run.MapFilename = request.MapFilename;
            run.Gambit = request.Gambit;
            run.Round = request.RoundsCompleted;
            run.TimeMs = request.TimeMs;
            run.Time50Ms = request.Time50Ms;
            run.AccountId0 = accountIds.ElementAtOrDefault(0);
            run.AccountId1 = accountIds.ElementAtOrDefault(1);
            run.AccountId2 = accountIds.ElementAtOrDefault(2);
            run.AccountId3 = accountIds.ElementAtOrDefault(3);
            run.AccountId4 = accountIds.ElementAtOrDefault(4);
            run.AccountId5 = accountIds.ElementAtOrDefault(5);
            run.AccountId6 = accountIds.ElementAtOrDefault(6);
            run.AccountId7 = accountIds.ElementAtOrDefault(7);
            run.AccountId8 = accountIds.ElementAtOrDefault(8);
            run.AccountId9 = accountIds.ElementAtOrDefault(9);
            customDb.AccountSurvivalMapRun.Update(run);

            await customDb.SaveChangesAsync();
            return Ok();
        }


        [Authorize]
        [HttpGet, Route("getLeaderboard/{LeaderboardName}")]
        public async Task<dynamic> getLeaderboard(SurvivalLeaderboards LeaderboardName, int Page, int PageSize)
        {
            var sql = "";
            var columnName = LeaderboardName switch
            {
                SurvivalLeaderboards.Completion => "",
                SurvivalLeaderboards.GamesPlayed => "[games_played]",
                SurvivalLeaderboards.TimePlayed => "[time_played_ms]",
                SurvivalLeaderboards.Kills => "[kills]",
                SurvivalLeaderboards.Deaths => "[deaths]",
                SurvivalLeaderboards.Revives => "[revives]",
                SurvivalLeaderboards.TimesRevived => "[times_revived]",
                SurvivalLeaderboards.WrenchKills => "[wrench_kills]",
                SurvivalLeaderboards.DualViperKills => "[dual_vipers_kills]",
                SurvivalLeaderboards.MagmaCannonKills => "[magma_cannon_kills]",
                SurvivalLeaderboards.ArbiterKills => "[arbiter_kills]",
                SurvivalLeaderboards.FusionKills => "[fusion_rifle_kills]",
                SurvivalLeaderboards.MineLauncherKills => "[mine_launcher_kills]",
                SurvivalLeaderboards.B6Kills => "[b6_kills]",
                SurvivalLeaderboards.HoloshieldKills => "[holoshield_kills]",
                SurvivalLeaderboards.FlailKills => "[scorpion_flail_kills]",
                _ => ""
            };

            switch (LeaderboardName)
            {
                case SurvivalLeaderboards.Completion:
                    {
                        sql = @"
                            SELECT
                                account_id,
                                account_name,
                                CAST(total_percent_completed*100 AS BIGINT) as [value],
                                leaderboard_rank
                            FROM dbo.GetSurvivalCompletionLeaderboard()
                            WHERE total_percent_completed > 0
                            ORDER BY leaderboard_rank ASC, account_id ASC
                            OFFSET @p0 ROWS 
                            FETCH NEXT @p1 ROWS ONLY;";
                        break;
                    }
                case SurvivalLeaderboards.GamesPlayed:
                case SurvivalLeaderboards.TimePlayed:
                case SurvivalLeaderboards.Kills:
                case SurvivalLeaderboards.Deaths:
                case SurvivalLeaderboards.Revives:
                case SurvivalLeaderboards.TimesRevived:
                case SurvivalLeaderboards.WrenchKills:
                case SurvivalLeaderboards.DualViperKills:
                case SurvivalLeaderboards.MagmaCannonKills:
                case SurvivalLeaderboards.ArbiterKills:
                case SurvivalLeaderboards.FusionKills:
                case SurvivalLeaderboards.MineLauncherKills:
                case SurvivalLeaderboards.B6Kills:
                case SurvivalLeaderboards.HoloshieldKills:
                case SurvivalLeaderboards.FlailKills:
                    {
                        sql = @$"
                            WITH ComputedRows AS (
                                SELECT 
                                    a.account_id,
                                    a.account_name,
                                    s.{columnName}
                                FROM [STATS].[account_survival_stat] s
                                JOIN [ACCOUNTS].[account] a ON a.account_id = s.account_id
                                WHERE a.is_active = 1 AND s.{columnName} > 0
                            )
                            SELECT
                                account_id as account_id,
                                account_name as account_name,
                                CAST({columnName} AS BIGINT) as [value],
                                RANK() OVER (ORDER BY {columnName} DESC) AS leaderboard_rank
                            FROM ComputedRows
                            ORDER BY {columnName} DESC, account_id ASC
                            OFFSET @p0 ROWS 
                            FETCH NEXT @p1 ROWS ONLY;";
                        break;
                    }
            }

            var startIndex = (Page - 1) * PageSize;
            var board = (await customDb.SurvivalLeaderboardRow.FromSqlRaw(sql, startIndex, PageSize)
                .ToListAsync())
                .Select(x => new SurvivalLeaderboardDTO()
                {
                    Index = x.LeaderboardRank,
                    StartIndex = startIndex,
                    AccountId = x.AccountId,
                    AccountName = x.AccountName,
                    StatValue = x.Value
                });

            return board;
        }

        [Authorize]
        [HttpGet, Route("getMapLeaderboard/{MapFilename}/{LeaderboardName}")]
        public async Task<dynamic> getMapLeaderboard(string MapFilename, SurvivalMapLeaderboards LeaderboardName, string? Gambit, int Page, int PageSize)
        {
            var isCoop = false;
            var sql = "";
            switch (LeaderboardName)
            {
                case SurvivalMapLeaderboards.Rank:
                    {
                        Gambit = null;
                        sql = @"
                            WITH ComputedRank AS (
                                SELECT 
                                    a.account_id,
		                            a.account_name,
		                            s.[rank] + (10000 * s.[prestige]) as [value]
                                FROM [STATS].[account_survival_map_stat] s
	                            JOIN [ACCOUNTS].[account] a ON a.account_id = s.account_id
	                            WHERE s.map_filename = @p0 AND a.is_active = 1 AND (s.[rank] > 0 OR s.[prestige] > 0)
                            )
                            SELECT
	                            CAST(account_id as NVARCHAR(32)) as account_ids,
	                            account_name as account_names,
	                            CAST([value] AS BIGINT) as [value],
                                RANK() OVER (ORDER BY [value] DESC) AS leaderboard_rank
                            FROM ComputedRank
                            ORDER BY [value] DESC, account_id ASC
                            OFFSET @p4 ROWS 
                            FETCH NEXT @p5 ROWS ONLY;";
                        break;
                    }
                case SurvivalMapLeaderboards.Completion:
                    {
                        Gambit = null;
                        sql = @$"
                            WITH ComputedRank AS (
                                SELECT 
                                    a.account_id,
		                            a.account_name,
		                            s.percent_completed as [value]
                                FROM [STATS].[account_survival_map_stat] s
	                            JOIN [ACCOUNTS].[account] a ON a.account_id = s.account_id
	                            WHERE s.map_filename = @p0 AND a.is_active = 1 AND s.percent_completed > 0
                            )
                            SELECT
	                            CAST(account_id as NVARCHAR(32)) as account_ids,
	                            account_name as account_names,
	                            CAST([value]*100 AS BIGINT) as [value],
                                Rank() OVER (ORDER BY [value] DESC) AS leaderboard_rank
                            FROM ComputedRank
                            ORDER BY [value] DESC, account_id ASC
                            OFFSET @p4 ROWS 
                            FETCH NEXT @p5 ROWS ONLY;";
                        break;
                    }
                case SurvivalMapLeaderboards.SoloBestRound:
                case SurvivalMapLeaderboards.CoopBestRound:
                    {
                        isCoop = LeaderboardName == SurvivalMapLeaderboards.CoopBestRound;
                        sql = @$"
                            WITH NormalizedGroups AS (
                                -- Step 1: Unpivot and Re-combine into a sorted string
                                SELECT 
                                    id,
                                    [round],
		                            created_dt,
                                    (SELECT STRING_AGG(account_id, ',') WITHIN GROUP (ORDER BY account_id)
                                     FROM (VALUES 
                                        (account_id_0), (account_id_1), (account_id_2), (account_id_3), 
                                        (account_id_4), (account_id_5), (account_id_6), (account_id_7), 
                                        (account_id_8), (account_id_9)
                                     ) AS v(account_id)
                                     WHERE account_id IS NOT NULL
                                    ) AS group_key,
                                    (SELECT STRING_AGG(a.[account_name], ', ') WITHIN GROUP (ORDER BY a.[account_name])
                                     FROM (VALUES (account_id_0), (account_id_1), (account_id_2), (account_id_3), 
                                                  (account_id_4), (account_id_5), (account_id_6), (account_id_7), 
                                                  (account_id_8), (account_id_9)) AS v(id)
                                     INNER JOIN [ACCOUNTS].[account] a ON v.id = a.account_id
                                     WHERE v.id IS NOT NULL) AS group_names
                                FROM [Ratchet_Deadlocked].[STATS].[account_survival_map_run]
	                            WHERE map_filename = @p0
		                              AND COALESCE(gambit, '') = COALESCE(@p1, '')
		                              AND player_count_at_start >= @p2
		                              AND player_count_at_start <= @p3
                            )
                            -- Step 2: Get the highest value per unique group
                            SELECT 
                                group_key AS [account_ids],
                                group_names AS [account_names],
                                CAST(MAX([round]) AS BIGINT) AS [value],
                                RANK() OVER (ORDER BY MAX([round]) DESC) AS leaderboard_rank
                            FROM NormalizedGroups
                            GROUP BY group_key, group_names
                            ORDER BY [value] DESC, MAX(created_dt) ASC
                            OFFSET @p4 ROWS 
                            FETCH NEXT @p5 ROWS ONLY;";
                        break;
                    }
                case SurvivalMapLeaderboards.SoloBestTime50:
                case SurvivalMapLeaderboards.CoopBestTime50:
                    {
                        isCoop = LeaderboardName == SurvivalMapLeaderboards.CoopBestTime50;
                        sql = @$"
                            WITH NormalizedGroups AS (
                                -- Step 1: Unpivot and Re-combine into a sorted string
                                SELECT 
                                    id,
                                    [time_50_ms],
		                            created_dt,
                                    (SELECT STRING_AGG(account_id, ',') WITHIN GROUP (ORDER BY account_id)
                                     FROM (VALUES 
                                        (account_id_0), (account_id_1), (account_id_2), (account_id_3), 
                                        (account_id_4), (account_id_5), (account_id_6), (account_id_7), 
                                        (account_id_8), (account_id_9)
                                     ) AS v(account_id)
                                     WHERE account_id IS NOT NULL
                                    ) AS group_key,
                                    (SELECT STRING_AGG(a.[account_name], ', ') WITHIN GROUP (ORDER BY a.[account_name])
                                     FROM (VALUES (account_id_0), (account_id_1), (account_id_2), (account_id_3), 
                                                  (account_id_4), (account_id_5), (account_id_6), (account_id_7), 
                                                  (account_id_8), (account_id_9)) AS v(id)
                                     INNER JOIN [ACCOUNTS].[account] a ON v.id = a.account_id
                                     WHERE v.id IS NOT NULL) AS group_names
                                FROM [Ratchet_Deadlocked].[STATS].[account_survival_map_run]
	                            WHERE map_filename = @p0
		                              AND COALESCE(gambit, '') = COALESCE(@p1, '')
		                              AND player_count_at_start >= @p2
		                              AND player_count_at_start <= @p3
		                              AND [time_50_ms] IS NOT NULL
                            )
                            -- Step 2: Get the highest value per unique group
                            SELECT 
                                group_key AS [account_ids],
                                group_names AS [account_names],
                                CAST(MIN([time_50_ms]) AS BIGINT) AS [value],
                                RANK() OVER (ORDER BY MIN([time_50_ms]) ASC) AS leaderboard_rank
                            FROM NormalizedGroups
                            GROUP BY group_key, group_names
                            ORDER BY [value] ASC, MAX(created_dt) ASC
                            OFFSET @p4 ROWS 
                            FETCH NEXT @p5 ROWS ONLY;";
                        break;
                    }
            }

            var startIndex = (Page - 1) * PageSize;
            var board = (await customDb.SurvivalMapLeaderboardRow.FromSqlRaw(sql, MapFilename, Gambit, isCoop ? 2 : 1, isCoop ? 9 : 1, startIndex, PageSize)
                .ToListAsync())
                .Select(x => new SurvivalMapLeaderboardDTO()
                {
                    Index = x.LeaderboardRank,
                    StartIndex = startIndex,
                    Gambit = Gambit,
                    MapFilename = MapFilename,
                    AccountIds = x.AccountIds.Split(',').Select(int.Parse).ToArray(),
                    AccountNames = x.AccountNames,
                    StatValue = x.Value
                });

            return board;
        }

    }
}
