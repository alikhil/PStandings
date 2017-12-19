using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Standings.Data.Migrations
{
    public partial class UpdateSubmissionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Contests_ContestPcmsId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Students_ContestPcmsId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ContestPcmsId",
                table: "Students");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions",
                columns: new[] { "ContestId", "ProblemId", "SubmitterId", "Time" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions");

            migrationBuilder.AddColumn<string>(
                name: "ContestPcmsId",
                table: "Students",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions",
                columns: new[] { "ContestId", "ProblemId", "SubmitterId" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_ContestPcmsId",
                table: "Students",
                column: "ContestPcmsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Contests_ContestPcmsId",
                table: "Students",
                column: "ContestPcmsId",
                principalTable: "Contests",
                principalColumn: "PcmsId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
