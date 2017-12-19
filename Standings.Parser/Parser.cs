using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;


using Standings.Parser.XmlModels;
using Standings.Data.Contexts;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Standings.Parser
{
    public class Parser: IDisposable
    {
        private ILogger Logger;
        private ParserSettings Settings;
        public Parser() 
        {
            Logger = Program.LoggerFabric.CreateLogger<Parser>();
            Settings = Program.Configuration
                .GetSection("Parsing")
                .Get<ParserSettings>();
        }

        public void Dispose()
        {
            (Logger as IDisposable)?.Dispose();
        }

        public void Start() 
        {
            using (var context = Program.ServiceProvider.GetService<PcmsContext>())
                foreach (var xmlFile in Directory.GetFiles(Settings.XmlDirectory))
                    ParseFile(xmlFile, context).Wait();
        }

        private async Task ParseFile(string xmlFile, PcmsContext context)
        {
             XmlSerializer serializer = new XmlSerializer(typeof(Standing));

            Logger.LogDebug($"parsing {xmlFile}");

            XmlReader reader = XmlReader.Create(xmlFile);
            var standings = (Standing)serializer.Deserialize(reader);
            var contestId = Path.GetFileNameWithoutExtension(xmlFile);
            Logger.LogDebug($"performing contest with name: {Standings.Contest.Name} and id: {contestId}");
            
            var contest = await context.Contests.FindAsync(contestId);
            if (contest == null)
            {
                Logger.LogInformation($"contest with id = {contestId} does not exist in db");
                var generation = contestId.StartsWith("algo-") ? "GR1" : "GR2";

                contest = Standings.Contest.ToDbModel(contestId).SetNameAndGeneration(contestId, generation);
                context.Contests.Add(contest);
                // marking is unchanged works very slow
                var allStudents = await context.Students.ToListAsync();
                var allProblems = await context.Problems.ToListAsync();
                var allSPs = await context.Problems
                    .Include(p => p.Submitters)
                    .SelectMany(s => s.Submitters)
                    .ToListAsync();

                foreach (var student in contest.Submissions.Select(s => s.Submitter).Distinct())
                {
                    if (allStudents.Find(s => s.Name == student.Name) != null)
                    {
                        context.Entry(student).State = EntityState.Unchanged;
                    }
                }

                foreach(var problem in contest.Submissions.Select(s => s.Problem).Distinct())
                {
                    if (allProblems.Find(p => p.Id == problem.Id) != null)
                    {
                        context.Entry(problem).State = EntityState.Unchanged;

                    }
                }

                foreach(var ps in contest.Submissions.SelectMany(s => s.Problem.Submitters).Distinct())
                {
                    if (allSPs.Find(p => p.ProblemId == ps.ProblemId && p.StudentId == ps.StudentId) != null)
                    {
                        context.Entry(ps).State = EntityState.Unchanged;

                    }
                }

                await context.SaveChangesAsync();
                Logger.LogInformation($"contest saved");
            }
        }
    }
}