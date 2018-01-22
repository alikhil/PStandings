using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Standings.Parser.XmlModels;
using Standings.Data.Contexts;
using System.Threading.Tasks;


namespace Standings.Parser
{
    public class Parser: IDisposable
    {
        private ILogger Logger;
        private ParserSettings Settings;
        private Timer Ticker;

        private PcmsContext Context;

        public Parser(PcmsContext context) 
        {
            Logger = Program.LoggerFabric.CreateLogger<Parser>();
            Settings = Program.Configuration
                .GetSection("Parsing")
                .Get<ParserSettings>();
            Context = context;
        }

        public void Dispose()
        {
            (Logger as IDisposable)?.Dispose();
            Ticker.Dispose();
            Context.Dispose();

        }

    
        private void OnTimerTick(object state)
        {
            Logger.LogInformation("Timer tick. Parsing all files...");
            ParseAllFiles();
        }

        private void ParseAllFiles()
        {
            foreach (var xmlFile in Directory.GetFiles(Settings.XmlDirectory, "*.xml"))
                ParseFile(xmlFile, Context).Wait();
        }
        public void Start() 
        {
            Logger.LogInformation("Parser started. Reading all contests...");
            ParseAllFiles();
            
            var delay = TimeSpan.FromSeconds(Settings.RefreshDelay);
            Ticker = new Timer(OnTimerTick, null, delay, delay);

        }

        private async Task ParseFile(string xmlFile, PcmsContext context)
        {
             XmlSerializer serializer = new XmlSerializer(typeof(Standing));

            Logger.LogDebug($"parsing {xmlFile}");

            XmlReader reader = XmlReader.Create(xmlFile);
            var standings = (Standing)serializer.Deserialize(reader);
            var contestId = Path.GetFileNameWithoutExtension(xmlFile);
            var contestHash = await CalculateMD5(xmlFile);
            Logger.LogDebug($"performing contest with name: {standings.Contest.Name} and id: {contestId}");
            
            var contest = await context.Contests.FindAsync(contestId);
            if (contest == null)
            {
                Logger.LogInformation($"contest with id = {contestId} does not exist in db");
                var generation = contestId.StartsWith("algo-") ? "GR1" : "GR2";

                contest = standings.Contest.ToDbModel(contestId).SetNameAndGeneration(contestId, generation);
                contest.Md5Checksum = contestHash;

                context.Contests.Add(contest);

                contest.Submissions
                    .Select(s => s.Submitter)
                    .Distinct()
                    .Select(s => new { Student = s, Exists = context.Students.Any(st => st.Name == s.Name) })
                    // .Select(o => { Console.Write($"{o.Student.Name}; exists: {o.Exists}"); return o; })
                    .Where(o => o.Exists)
                    .Select(o => o.Student)
                    .SetEntityState(context, EntityState.Unchanged);

                await context.SaveChangesAsync();
                Logger.LogInformation($"contest saved");
            } else 
            {
                if (contest.Md5Checksum != contestHash)
                {
                    Logger.LogInformation($"rewriting contest with id = {contestId}");
                    var generation = contestId.StartsWith("algo-") ? "GR1" : "GR2";

                    var newContest = standings.Contest.ToDbModel(contestId).SetNameAndGeneration(contestId, generation);
                    contest.Md5Checksum = contestHash;
                    contest.LastUpdate = DateTime.Now;

                    context.Entry(contest).State = EntityState.Modified;

                    // if contest exists then there are limited number of reasons 
                    // why it pushed to update
                    // 1) new problem was added
                    // 2) new submissions
                    // 3) new student_problems
                    // 4) new students

                    newContest.Problems // marking existing problems as unchanged
                        .Select(p => ( problem: p, exists: context.Problems.Find(p.Id) != null ))
                        .Where(o => !o.exists)
                        .Select(o => o.problem)
                        .SetEntityState(context ,EntityState.Added);

                    newContest.Submissions
                        .Select(s => (submission: s, exists: context.Submissions.Find(s.Contest.PcmsId, s.Problem.Id, s.Submitter.Name, s.Time) != null))
                        .Where(t => !t.exists)
                        .Select(t => t.submission)
                        .SetEntityState(context, EntityState.Added);
                    
                    var nulll = newContest.Submissions.FirstOrDefault(s => s.Problem.Submitters == null);
                    if (nulll != null)
                        Logger.LogWarning($"{nulll.Submitter.Name} has shitty submition");
                    newContest.Submissions.SelectMany(s => s.Problem.Submitters ?? new Data.Models.ProblemStudent[] {})
                        .Distinct()
                        .Where(s => s.Student != null)
                        // .Select(o => { Console.WriteLine($"{o.Problem?.Id ?? "pidnull"}-{o.Student?.Name ?? "sidnull"}") ; return o; })
                        .Select(sp => (studentProblem: sp, exists: context.ProblemStudent.Find(sp.Student.Name, sp.Problem.Id) != null ))
                        .Where(t => !t.exists)
                        .Select(t => t.studentProblem)
                        .SetEntityState(context, EntityState.Added);

                    newContest.Submissions.Select(s => s.Submitter)
                        .Select(s => (student: s, exists: context.Students.Find(s.Name) != null))
                        .Where(t => !t.exists)
                        .Select(t => t.student)
                        .SetEntityState(context, EntityState.Added);

                    await context.SaveChangesAsync();
                    Logger.LogInformation($"contest with id = {contestId} rewrited");
                    
                }
                else
                {
                    Logger.LogInformation($"contest with id = {contestId} not changed. did not nothing");
                }
            }
        }

        private static async Task<string> CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var reader = XmlReader.Create(filename, new XmlReaderSettings() { Async = true }))
                {
                    reader.ReadToFollowing("contest");
                    var xml = await reader.ReadInnerXmlAsync();
                    var hash = md5.ComputeHash(Encoding.Default.GetBytes(xml));
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}