using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

using DM = Standings.Data.Models;
using System;

namespace Standings.Parser
{
    public static class ModelExtensions
    {
        public static DM.Contest ToDbModel(this XmlModels.Contest xcontest, string problemIdPrefix)
        {
            var contest = new  DM.Contest();
            contest.Name = xcontest.Name;
            var problems = xcontest.Challenge.Problems
                .Select(p => new DM.Problem().SetName(p.Name).SetAlias(p.Alias));

            // var students = xcontest.Sessions
            //     .Select(s => 
            //         new DM.Student()
            //             .SetName(s.Party)
            //             .SetSubmissions(s.Problems.SelectMany(p => 
            //                                                 p.Runs.Select(r => 
            //                                                     new DM.Submission()
            //                                                         .SetFromRun(r)
            //                                                         .SetProblem(problems.First(pr => pr.Alias == p.Alias))
            //                                                         )
            //                                                 )
            //                             )
            //             );
            var studentDict = new Dictionary<string, DM.Student>();
            Func<string, DM.Student> getStudent = (string name) => 
            {
                if (!studentDict.ContainsKey(name))
                    studentDict[name] = new DM.Student().SetName(name);
                return studentDict[name];
            };

            var problms = xcontest.Sessions
                .SelectMany(s => 
                    s.Problems.Select(p => 
                        new DM.Problem()
                            .SetAlias(p.Alias)
                            .SetId(problemIdPrefix + p.Id)
                            .SetSubmitters(new [] { getStudent(s.Party) })
                            .SetSubmissions(p.Runs
                                    .TakeWhile(run => run.Accepted == "no")
                                    .Concat(p.Runs.Where(run => run.Accepted == "yes").Take(1))
                                    .Select(r => new DM.Submission()
                                        .SetFromRun(r)
                                        .SetContest(contest)
                                        .SetSubmitter(getStudent(s.Party))))))
                .GroupBy(p => p.Alias)
                .Select(g => new DM.Problem()
                                .SetContest(contest)
                                .SetAlias(g.First().Alias)
                                .SetId(g.First().Id)
                                .SetSubmissions(g.SelectMany(p => p.Submissions))
                                .SetSubmitters(g.SelectMany(p => p.Submitters.Select(sp => sp.Student))))
                .Select(p => p.SetName(problems.First(pr => pr.Alias == p.Alias).Name));
            
            contest.Problems = problms.Select(pr => pr.SetSubmissions(pr.Submissions.Select(s => s.SetProblem(pr)))).ToList();
            Console.WriteLine(contest.Problems.Select(p => p.Submissions.Select(s => $"{s.Contest.PcmsId}{s.Problem.Id}{s.Submitter.Name}{s.Time}").Distinct().Count()).Sum());
            // foreach(var problem in contest.Problems)
            contest.Submissions = contest.Problems.SelectMany(p => p.Submissions).ToList();
            return contest;
        }

        public static DM.Problem SetContest(this DM.Problem problem, DM.Contest contest)
        {
            problem.Contest = contest;
            return problem;
        }
        public static DM.Contest SetNameAndGeneration(this DM.Contest contest, string name, string generation) 
        {
            contest.PcmsId = name;
            contest.Generation = generation;
            return contest;
        }
        
        public static DM.Problem SetSubmitters(this DM.Problem problem, IEnumerable<DM.Student> submitters)
        {
            problem.Submitters = submitters.Select(s => new DM.ProblemStudent() { Problem = problem, Student = s }).ToList();
            return problem;
        }

        public static DM.Problem SetSubmissions(this DM.Problem problem, IEnumerable<DM.Submission> submissions)
        {
            problem.Submissions = submissions.ToList();
            return problem;
        }

        public static DM.Student SetSubmissions(this DM.Student student, IEnumerable<DM.Submission> submissions)
        {
            student.Submissions = submissions.ToList();
            return student;
        }

        public static DM.Submission SetSubmitter(this DM.Submission submission, DM.Student submitter)
        {
            submission.Submitter = submitter;
            return submission;
        }
        public static DM.Problem SetId(this DM.Problem problem, string id)
        {
            problem.Id = id;
            return problem;
        }

        public static DM.Problem SetAlias(this DM.Problem problem, string alias)
        {
            problem.Alias = alias;
            return problem;
        }

        public static DM.Problem SetName(this DM.Problem problem, string name)
        {
            problem.Name = name;
            return problem;
        }

        public static DM.Submission SetContest(this DM.Submission submission, DM.Contest contest)
        {
            submission.Contest = contest;
            return submission;
        }
        public static DM.Submission SetProblem(this DM.Submission submission, DM.Problem problem)
        {
            submission.Problem = problem;
            return submission;
        }
        public static DM.Submission SetFromRun(this DM.Submission submission, XmlModels.Run run)
        {
            submission.IsAccepted = run.Accepted == "yes";
            submission.Time = (long.Parse(run.Time) + Program.Configuration.GetValue<long>("Parsing:ContestStartTime"));
            return submission;
        }
        public static DM.Student SetName(this DM.Student student, string name)
        {
            student.Name = name;
            return student;
        }
    }
}