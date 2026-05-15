using Horizon.Database.Entities;
using Horizon.Database.Services;
using Horizon.Middleware.Plugin.Deadlocked.DTO;
using Horizon.Middleware.Plugin.Deadlocked.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class ForgeCgmController : ControllerBase
    {
        private Ratchet_DeadlockedContext db;
        private CustomDbContext customDb;
        private IAuthService authService;
        public ForgeCgmController(Ratchet_DeadlockedContext _db, CustomDbContext _customDb, IAuthService _authService)
        {
            db = _db;
            customDb = _customDb;
            authService = _authService;
        }

        [Authorize]
        [HttpGet, Route("getMap/{mapFilename}")]
        public async Task<dynamic> getMap(string mapFilename)
        {
            return await (from m in customDb.DimForgeCgmMaps
                          where m.MapFilename == mapFilename
                          select new ForgeCgmMapDTO()
                          {
                              Name = m.Name,
                              MapFilename = m.MapFilename,
                              SharedRankCode = m.SharedRankCode,
                              Metadata = m.Metadata,
                          }).FirstOrDefaultAsync() ?? new ForgeCgmMapDTO() { MapFilename = mapFilename };
        }

        [Authorize("database")]
        [HttpPost, Route("updateMap")]
        public async Task<dynamic> updateMap([FromBody] ForgeCgmMapDTO request)
        {
            if (request == null)
                return BadRequest("Missing request body");
            if (string.IsNullOrEmpty(request.Name))
                return BadRequest("Missing Name");
            if (string.IsNullOrEmpty(request.MapFilename))
                return BadRequest("Missing MapFilename");

            // store empty string as null
            var normalizedSharedRankCode = string.IsNullOrEmpty(request.SharedRankCode) ? null : request.SharedRankCode;

            var map = await customDb.DimForgeCgmMaps.FirstOrDefaultAsync(s => s.MapFilename == request.MapFilename);
            if (map == null)
            {
                map = new DimForgeCgmMaps()
                {
                    MapFilename = request.MapFilename,
                    Name = request.Name,
                    SharedRankCode = normalizedSharedRankCode,
                    Metadata = request.Metadata,
                    ModifiedDt = DateTime.UtcNow
                };

                customDb.DimForgeCgmMaps.Add(map);
            }
            else
            {
                map.Name = request.Name;
                map.SharedRankCode = normalizedSharedRankCode;
                map.Metadata = request.Metadata;
                map.ModifiedDt = DateTime.UtcNow;

                customDb.DimForgeCgmMaps.Update(map);
            }

            await customDb.SaveChangesAsync();
            return new ForgeCgmMapDTO()
            {
                MapFilename = map.MapFilename,
                Name = map.Name,
                Metadata = map.Metadata,
            };
        }

        [Authorize]
        [HttpGet, Route("getMapAccountStats")]
        public async Task<dynamic> getAccountStats(int AccountId, string MapFilename)
        {
            if (AccountId == 0)
                return BadRequest("Missing AccountId");
            if (string.IsNullOrEmpty(MapFilename))
                return BadRequest("Missing MapFilename");

            var accountStats = (await customDb.ForgeCgmMapAccountStat.FromSqlRaw("EXEC [dbo].[ForgeCgmGetAccountMapStats] @p0, @p1", AccountId, MapFilename).ToListAsync()).FirstOrDefault();
            if (accountStats == null)
            {
                return new ForgeCgmMapAccountStatDTO()
                {
                    AccountId = AccountId,
                    MapFilename = MapFilename,
                    TrackedStats = new long[8]
                };
            }

            return new ForgeCgmMapAccountStatDTO()
            {
                AccountId = accountStats.AccountId,
                MapFilename = accountStats.MapFilename,
                Rank = accountStats.Rank,
                Wins = accountStats.Wins,
                Losses = accountStats.Losses,
                GamesPlayed = accountStats.GamesPlayed,
                TimePlayedMs = accountStats.TimePlayedMs,
                TrackedStats = new long[]
                {
                    accountStats.TrackedStat1,
                    accountStats.TrackedStat2,
                    accountStats.TrackedStat3,
                    accountStats.TrackedStat4,
                    accountStats.TrackedStat5,
                    accountStats.TrackedStat6,
                    accountStats.TrackedStat7,
                    accountStats.TrackedStat8,
                }
            };
        }

        [Authorize]
        [HttpGet, Route("getMapAggregatedAccountStats")]
        public async Task<dynamic> getAggregatedAccountStats(int AccountId, string MapFilename)
        {
            if (AccountId == 0)
                return BadRequest("Missing AccountId");
            if (string.IsNullOrEmpty(MapFilename))
                return BadRequest("Missing MapFilename");

            var accountStats = (await customDb.ForgeCgmMapAggregatedAccountStats.FromSqlRaw("EXEC [dbo].[ForgeCgmGetAggregatedAccountMapStats] @p0, @p1", AccountId, MapFilename).ToListAsync()).FirstOrDefault();
            if (accountStats == null)
            {
                return new ForgeCgmMapAggregatedAccountStatDTO()
                {
                    AccountId = AccountId,
                    TrackedStats = new long[8]
                };
            }

            return new ForgeCgmMapAggregatedAccountStatDTO()
            {
                AccountId = accountStats.AccountId,
                Rank = accountStats.Rank,
                Ranking = accountStats.Ranking,
                Wins = accountStats.Wins,
                Losses = accountStats.Losses,
                GamesPlayed = accountStats.GamesPlayed,
                TimePlayedMs = accountStats.TimePlayedMs,
                TrackedStats = new long[]
                {
                    accountStats.TrackedStat1,
                    accountStats.TrackedStat2,
                    accountStats.TrackedStat3,
                    accountStats.TrackedStat4,
                    accountStats.TrackedStat5,
                    accountStats.TrackedStat6,
                    accountStats.TrackedStat7,
                    accountStats.TrackedStat8,
                }
            };
        }

        [Authorize("database")]
        [HttpPost, Route("updateMapAccountStats")]
        public async Task<dynamic> updateMapAccountStats([FromBody] ForgeCgmMapAccountStatDTO request)
        {
            if (request == null)
                return BadRequest("Missing request body");
            if (request.AccountId == 0)
                return BadRequest("Missing AccountId");
            if (string.IsNullOrEmpty(request.MapFilename))
                return BadRequest("Missing MapFilename");

            var map = await customDb.DimForgeCgmMaps.FirstOrDefaultAsync(s => s.MapFilename == request.MapFilename);
            if (map == null)
                return NotFound();

            var stats = await customDb.ForgeCgmMapAccountStat.FirstOrDefaultAsync(s => s.AccountId == request.AccountId && s.MapFilename == request.MapFilename);
            if (stats == null)
            {
                stats = new ForgeCgmMapAccountStat()
                {
                    AccountId = request.AccountId,
                    MapFilename = request.MapFilename,
                    Rank = request.Rank,
                    Wins = request.Wins,
                    Losses = request.Losses,
                    GamesPlayed = request.GamesPlayed,
                    TimePlayedMs = request.TimePlayedMs,
                    TrackedStat1 = request.TrackedStats.ElementAtOrDefault(0),
                    TrackedStat2 = request.TrackedStats.ElementAtOrDefault(1),
                    TrackedStat3 = request.TrackedStats.ElementAtOrDefault(2),
                    TrackedStat4 = request.TrackedStats.ElementAtOrDefault(3),
                    TrackedStat5 = request.TrackedStats.ElementAtOrDefault(4),
                    TrackedStat6 = request.TrackedStats.ElementAtOrDefault(5),
                    TrackedStat7 = request.TrackedStats.ElementAtOrDefault(6),
                    TrackedStat8 = request.TrackedStats.ElementAtOrDefault(7),
                    ModifiedDt = DateTime.UtcNow
                };

                customDb.ForgeCgmMapAccountStat.Add(stats);
            }
            else
            {
                stats.Rank = request.Rank;
                stats.Wins = request.Wins;
                stats.Losses = request.Losses;
                stats.GamesPlayed = request.GamesPlayed;
                stats.TimePlayedMs = request.TimePlayedMs;
                stats.TrackedStat1 = request.TrackedStats.ElementAtOrDefault(0);
                stats.TrackedStat2 = request.TrackedStats.ElementAtOrDefault(1);
                stats.TrackedStat3 = request.TrackedStats.ElementAtOrDefault(2);
                stats.TrackedStat4 = request.TrackedStats.ElementAtOrDefault(3);
                stats.TrackedStat5 = request.TrackedStats.ElementAtOrDefault(4);
                stats.TrackedStat6 = request.TrackedStats.ElementAtOrDefault(5);
                stats.TrackedStat7 = request.TrackedStats.ElementAtOrDefault(6);
                stats.TrackedStat8 = request.TrackedStats.ElementAtOrDefault(7);
                stats.ModifiedDt = DateTime.UtcNow;

                customDb.ForgeCgmMapAccountStat.Update(stats);
            }

            // Update rank for all maps with the same shared rank code
            if (!string.IsNullOrEmpty(map.SharedRankCode))
            {
                // The variables are parameterized automatically by EF Core
                await customDb.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC [dbo].[ForgeCgmSetSharedAccountRank] 
                        @account_id = {request.AccountId}, 
                        @shared_rank_code = {map.SharedRankCode}, 
                        @new_rank = {request.Rank}");
            }

            await customDb.SaveChangesAsync();
            return await getAccountStats(request.AccountId, request.MapFilename);
        }

    }
}
