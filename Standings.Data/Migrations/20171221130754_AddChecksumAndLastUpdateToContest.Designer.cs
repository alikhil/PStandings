﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Standings.Data.Contexts;
using System;

namespace Standings.Data.Migrations
{
    [DbContext(typeof(PcmsContext))]
    [Migration("20171221130754_AddChecksumAndLastUpdateToContest")]
    partial class AddChecksumAndLastUpdateToContest
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Standings.Data.Models.Contest", b =>
                {
                    b.Property<string>("PcmsId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Generation");

                    b.Property<bool>("Hidden");

                    b.Property<DateTime>("LastUpdate");

                    b.Property<string>("Md5Checksum");

                    b.Property<string>("Name");

                    b.HasKey("PcmsId");

                    b.ToTable("Contests");
                });

            modelBuilder.Entity("Standings.Data.Models.Problem", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Alias");

                    b.Property<string>("ContestPcmsId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("ContestPcmsId");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("Standings.Data.Models.ProblemStudent", b =>
                {
                    b.Property<string>("StudentId");

                    b.Property<string>("ProblemId");

                    b.HasKey("StudentId", "ProblemId");

                    b.HasIndex("ProblemId");

                    b.ToTable("ProblemStudent");
                });

            modelBuilder.Entity("Standings.Data.Models.Student", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Name");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Standings.Data.Models.Submission", b =>
                {
                    b.Property<string>("ContestId");

                    b.Property<string>("ProblemId");

                    b.Property<string>("SubmitterId");

                    b.Property<long>("Time");

                    b.Property<bool>("IsAccepted");

                    b.HasKey("ContestId", "ProblemId", "SubmitterId", "Time");

                    b.HasIndex("ProblemId");

                    b.HasIndex("SubmitterId");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("Standings.Data.Models.Problem", b =>
                {
                    b.HasOne("Standings.Data.Models.Contest", "Contest")
                        .WithMany("Problems")
                        .HasForeignKey("ContestPcmsId");
                });

            modelBuilder.Entity("Standings.Data.Models.ProblemStudent", b =>
                {
                    b.HasOne("Standings.Data.Models.Problem", "Problem")
                        .WithMany("Submitters")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Standings.Data.Models.Student", "Student")
                        .WithMany("SubmittedProblems")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Standings.Data.Models.Submission", b =>
                {
                    b.HasOne("Standings.Data.Models.Contest", "Contest")
                        .WithMany("Submissions")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Standings.Data.Models.Problem", "Problem")
                        .WithMany("Submissions")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Standings.Data.Models.Student", "Submitter")
                        .WithMany("Submissions")
                        .HasForeignKey("SubmitterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
