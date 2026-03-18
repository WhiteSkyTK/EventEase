using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEG5WjhmfKvjjw8e9raR5Fz2FFgieTMQddiiVd+lGNjjQQ37acmMretgZ547KX4UN1w==");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 2,
                column: "Password",
                value: "AQAAAAIAAYagAAAAED+hvEw62MOxISLrGaCxt/jjP++vp7Rbf6LvCaE9Nxw2LaHZiNOyqLswykOGFnbSQw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJ9V6X8Y9QpY4z5L7vN3nK9wR2mB5tC8vX1zL0P9Q8W7E6R5");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "StaffId",
                keyValue: 2,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEB7V5X9Y8QpX4z4L6vN2nK8wR1mB4tC7vX0zL9P8Q7W6E5R4");
        }
    }
}
