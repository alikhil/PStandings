using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Standings.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    PcmsId = table.Column<string>(nullable: false),
                    Generation = table.Column<string>(nullable: true),
                    Hidden = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.PcmsId);
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Alias = table.Column<string>(nullable: true),
                    ContestPcmsId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_Contests_ContestPcmsId",
                        column: x => x.ContestPcmsId,
                        principalTable: "Contests",
                        principalColumn: "PcmsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    ContestPcmsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Students_Contests_ContestPcmsId",
                        column: x => x.ContestPcmsId,
                        principalTable: "Contests",
                        principalColumn: "PcmsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProblemStudent",
                columns: table => new
                {
                    StudentId = table.Column<string>(nullable: false),
                    ProblemId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemStudent", x => new { x.StudentId, x.ProblemId });
                    table.ForeignKey(
                        name: "FK_ProblemStudent_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemStudent_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    ContestId = table.Column<string>(nullable: false),
                    ProblemId = table.Column<string>(nullable: false),
                    SubmitterId = table.Column<string>(nullable: false),
                    IsAccepted = table.Column<bool>(nullable: false),
                    Time = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => new { x.ContestId, x.ProblemId, x.SubmitterId });
                    table.ForeignKey(
                        name: "FK_Submissions_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "PcmsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Students_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Students",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ContestPcmsId",
                table: "Problems",
                column: "ContestPcmsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemStudent_ProblemId",
                table: "ProblemStudent",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ContestPcmsId",
                table: "Students",
                column: "ContestPcmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ProblemId",
                table: "Submissions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_SubmitterId",
                table: "Submissions",
                column: "SubmitterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProblemStudent");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Problems");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Contests");
        }
    }
}
