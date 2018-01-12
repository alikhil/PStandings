using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

using Standings.Repositories;
using Standings.Models;
using Standings.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Standings.Controllers
{
    [Route("api/pcms/{generation}")]
    public class ContestController
    {
        private readonly PcmsContext DataContext;
        private readonly ILogger Logger;
        public ContestController(ILoggerFactory loggerFactory, PcmsContext pcmsContext)
        {
            DataContext = pcmsContext;
            Logger = loggerFactory.CreateLogger<ContestController>();
        }


        [HttpGet("contests")]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "generation" })]
        public async Task<IEnumerable<Contest>> GetContests(string generation)
        {
            return await Task.FromResult(DataContext.Contests
                .Where(c => c.Generation == generation)
                .Select(ToContest));
        }

        public Contest ToContest(Standings.Data.Models.Contest contest)
        {
            return new Contest()
            {
                Name = contest.Name,
                Id = contest.PcmsId
            };
        }

        [HttpGet("contest/{contestId}")]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new string[] { "generation", "contestId" })]
        public async Task<ContestStats> GetContestStats(string generation, string contestId)
        {
            var contest = DataContext.Contests
                .Include(c => c.Problems)
                .Include(c => c.Submissions)
                .Where(c => c.PcmsId == contestId && c.Generation == generation)
                .FirstOrDefault();
            if (contest == null)
            {
                Logger.LogError($"Contest {contestId}:{generation} not found");
                return null;
            }
            var stats = new ContestStats();
            stats.Name = contest.Name;
            stats.Problems = contest.Problems
                .Select(p => new Problem() { Name = p.Name, Alias = p.Alias})
                .OrderBy(p => p.Alias)
                .ToList();

            stats.Results = contest.Submissions
                .Select(s => new { ProblemAlias = s.Problem.Alias, IsAccepted = s.IsAccepted, s.SubmitterId })
                .GroupBy(o => o.SubmitterId)
                .Select(g => new 
                { 
                    Name = g.Key,
                    ProblemsStatusDict = g.GroupBy(e => e.ProblemAlias)
                        .ToDictionary(gr => gr.Key, gr => new { Accepted = gr.Any(e => e.IsAccepted), Attempts = gr.Count() })})
                .Select(o => new ContestUserResult()
                { 
                    Name = o.Name,
                    ProblemsStatus = stats.Problems
                        .Select(p => o.ProblemsStatusDict.GetValueOrDefault(p.Alias, null) ?? new { Accepted = false, Attempts = 0 })
                        .Select(ob => ob.Attempts == 0 ? "." : ((ob.Accepted ? "+" : "-") + (ob.Attempts - (ob.Accepted ? -1 : 0)).ToString()))
                        .ToList(),
                    TotalSolved = o.ProblemsStatusDict.Count(kv => kv.Value.Accepted)})
                .OrderByDescending(r => r.TotalSolved)
                .ToList();
            return await Task.FromResult(stats);
        }

        [HttpGet("submissions")]
        public async Task<IEnumerable<Submission>> GetSubmissons(string generation, long startDate, long endDate, string prefix = "", int count = 5, int page = 0)
        {
            return await Task.FromResult(DataContext.Submissions
                .Include(s => s.Contest)
                .Include(s => s.Problem)
                .Include(s => s.Submitter)
                .Where(s => string.IsNullOrEmpty(prefix) || s.Submitter.Name.StartsWith(prefix))
                .Where(s => s.Contest.Generation == generation)
                .Where(s => s.Time <= endDate * 1000 && s.Time >= startDate * 1000)
                .OrderByDescending(s => s.Time)
                .Skip(page * count)
                .Take(count)
                .Select(ToSubmittion));
        }

        private Submission ToSubmittion(Standings.Data.Models.Submission submission)
        {
            return new Submission()
            {
                Time = submission.Time / 1000,
                Contest = submission.Contest.Name,
                ProblemAlias = submission.Problem.Alias,
                ProblemTitle = submission.Problem.Name,
                Accepted = submission.IsAccepted,
                Username = submission.Submitter.Name,
            };
        }
        [HttpGet("standings")]
        [ResponseCache(Duration = 45, VaryByQueryKeys = new string[] { "generation" })]
        public async Task<StandingStats> GetStandingStats(string generation)
        {
            var contests = DataContext.Contests
                .Include(c => c.Submissions)
                .Where(c => c.Generation == generation);
        
            var stats = new StandingStats();
            stats.Contests = contests
                .Select(c => c.Name)
                .OrderBy(name => name)
                .ToList();

            stats.Results = contests.Select(c => c.Submissions
                .GroupBy(s => s.SubmitterId)
                .Select(g => new {
                        StudentId = g.First().SubmitterId,
                        Solved = g.Count(s => s.IsAccepted),
                        ContestName = c.Name
                    }))
                .SelectMany(c => c.Select(s => s))
                .GroupBy(d => d.StudentId)
                .Select(d => new {
                            Name = d.Key, 
                            TotalSolved = d.Sum(e => e.Solved), 
                            ContestSolved = d.ToDictionary(e => e.ContestName, e => e.Solved)
                        })
                .Select(v => new UserResult(v.Name,
                        v.TotalSolved, 
                        stats.Contests.Select(c => v.ContestSolved.GetValueOrDefault(c, 0)).ToList()))
                .OrderByDescending(r => r.TotalSolved)
                .ToList();
            
            return await Task.FromResult(stats);
        }



        [HttpGet("analytics/common")]
        public async Task<object> GetCommonAnalytics(string generation, long startDate, long endDate, string type, string prefix = "")
        {
            Func<Data.Models.Submission, string> getGroupper = (s) =>
            {
                var date = JavaTimeStampToDateTime(s.Time);
                if (type == "month")
                    return date.ToString("yyyy-MM");
                else // if type == "day"
                    return date.ToString("yyyy-MM-dd");
            };
            var groups = DataContext.Submissions
                .Where(s => s.Time >= startDate * 1000 && s.Time <= endDate * 1000)
                .Where(s => string.IsNullOrEmpty(prefix) || s.SubmitterId.StartsWith(prefix))
                .GroupBy(s => getGroupper(s))
                .Select(g => new {
                    Key = g.Key,
                    Success = g.Count(s => s.IsAccepted),
                    All = g.Count()
                })
                .OrderBy(g => g.Key);

            var result = new
            { 
                type, 
                keys = groups.Select(g => g.Key),
                all = groups.Select(g => g.All),
                success = groups.Select(g => g.Success)
            };
            return await Task.FromResult(result);
        }

        public static DateTime JavaTimeStampToDateTime( double javaTimeStamp )
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}