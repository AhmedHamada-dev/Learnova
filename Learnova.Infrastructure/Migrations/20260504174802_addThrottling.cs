using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addThrottling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForgetPasswordCountInWindow",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ForgetPasswordWindowStartedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastForgetPasswordAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOtpSentAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OtpSendCountInWindow",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpWindowStartedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForgetPasswordCountInWindow",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ForgetPasswordWindowStartedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastForgetPasswordAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastOtpSentAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpSendCountInWindow",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpWindowStartedAt",
                table: "AspNetUsers");
        }
    }
}
