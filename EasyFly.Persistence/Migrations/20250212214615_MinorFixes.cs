﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyFly.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MinorFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AspNetUsers");
        }
    }
}
