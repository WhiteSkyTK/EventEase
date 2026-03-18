using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class FinalStaffHashing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffId);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    VenueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.VenueId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "StaffId", "Email", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin@eventease.com", "AQAAAAIAAYagAAAAEJ9V6X8Y9QpY4z5L7vN3nK9wR2mB5tC8vX1zL0P9Q8W7E6R5", "Admin" },
                    { 2, "specialist@eventease.com", "AQAAAAIAAYagAAAAEB7V5X9Y8QpX4z4L6vN2nK8wR1mB4tC7vX0zL9P8Q7W6E5R4", "Specialist" }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "VenueId", "Capacity", "ImageUrl", "Location", "VenueName" },
                values: new object[,]
                {
                    { 1, 500, "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=800", "123 Marble Ave, Cape Town", "The Grand Ballroom" },
                    { 2, 150, "https://images.unsplash.com/photo-1523438885200-e635ba2c371e?w=800", "45 Ocean View, Durban", "Sunset Garden" },
                    { 3, 1000, "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800", "88 Innovation Dr, Sandton", "Tech Hub Plaza" },
                    { 4, 30, "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800", "12 Main St, Pretoria", "Cozy Corner Café" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "Description", "EndDateTime", "EventName", "ImageUrl", "StartDateTime", "VenueId" },
                values: new object[,]
                {
                    { 1, "Formal dinner and auction.", new DateTime(2026, 5, 10, 23, 59, 0, 0, DateTimeKind.Unspecified), "Annual Charity Gala", "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?q=80&w=1000", new DateTime(2026, 5, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Technical conference.", new DateTime(2026, 6, 15, 17, 0, 0, 0, DateTimeKind.Unspecified), "Cloud Dev Summit", "https://images.unsplash.com/photo-1540575861501-7ce0e1d1aa6f?q=80&w=1000", new DateTime(2026, 6, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 3, "Private ceremony.", new DateTime(2026, 7, 20, 22, 0, 0, 0, DateTimeKind.Unspecified), "Smith Wedding", "https://images.unsplash.com/photo-1519741497674-611481863552?q=80&w=1000", new DateTime(2026, 7, 20, 14, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingDate", "EventId", "VenueId" },
                values: new object[] { 1, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventId",
                table: "Bookings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VenueId",
                table: "Bookings",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueId",
                table: "Events",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
