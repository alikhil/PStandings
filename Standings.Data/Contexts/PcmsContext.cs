using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Standings.Data.Models;

namespace Standings.Data.Contexts
{
    public class PcmsContext : DbContext
    {
        public PcmsContext(DbContextOptions<PcmsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Submission>()
                .HasKey(c => new { c.ContestId, c.ProblemId, c.SubmitterId, c.Time });

            modelBuilder.Entity<ProblemStudent>()
                .HasKey(e => new { e.StudentId, e.ProblemId });

            modelBuilder.Entity<ProblemStudent>()
                .HasOne(ps => ps.Problem)
                .WithMany(p => p.Submitters)
                .HasForeignKey(ps => ps.ProblemId);
            
            modelBuilder.Entity<ProblemStudent>()
                .HasOne(ps => ps.Student)
                .WithMany(s => s.SubmittedProblems)
                .HasForeignKey(ps => ps.StudentId);
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProblemStudent> ProblemStudent { get; set; }

    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PcmsContext>
    {
        public PcmsContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<PcmsContext>();
            var connectionString = configuration.GetConnectionString("StandingsConnectionString");
            builder.UseSqlServer(connectionString);
            return new PcmsContext(builder.Options);
        }
    }
}