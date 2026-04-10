using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class InitialFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    EventTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.EventTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
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
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "EventTypeId",
                        onDelete: ReferentialAction.Cascade);
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
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerSurname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false)
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
                table: "EventTypes",
                columns: new[] { "EventTypeId", "TypeName" },
                values: new object[,]
                {
                    { 1, "Wedding" },
                    { 2, "Conference" },
                    { 3, "Concert" },
                    { 4, "Gala" }
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "StaffId", "Email", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin@eventease.com", "AQAAAAIAAYagAAAAEG5WjhmfKvjjw8e9raR5Fz2FFgieTMQddiiVd+lGNjjQQ37acmMretgZ547KX4UN1w==", "Admin" },
                    { 2, "specialist@eventease.com", "AQAAAAIAAYagAAAAED+hvEw62MOxISLrGaCxt/jjP++vp7Rbf6LvCaE9Nxw2LaHZiNOyqLswykOGFnbSQw==", "Specialist" }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "VenueId", "Capacity", "ImageUrl", "IsAvailable", "Location", "VenueName" },
                values: new object[,]
                {
                    { 1, 500, "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=800", true, "123 Marble Ave, Cape Town", "The Grand Ballroom" },
                    { 2, 150, "https://images.unsplash.com/photo-1523438885200-e635ba2c371e?w=800", true, "45 Ocean View, Durban", "Sunset Garden" },
                    { 3, 1000, "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800", true, "88 Innovation Dr, Sandton", "Tech Hub Plaza" },
                    { 4, 30, "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800", true, "12 Main St, Pretoria", "Cozy Corner Café" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "Description", "EndDateTime", "EventName", "EventTypeId", "ImageUrl", "StartDateTime", "VenueId" },
                values: new object[,]
                {
                    { 1, "Formal dinner and auction.", new DateTime(2026, 5, 10, 23, 59, 0, 0, DateTimeKind.Unspecified), "Annual Charity Gala", 4, "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?q=80&w=1000", new DateTime(2026, 5, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Technical conference.", new DateTime(2026, 6, 15, 17, 0, 0, 0, DateTimeKind.Unspecified), "Cloud Dev Summit", 2, "https://images.unsplash.com/photo-1540575861501-7ce0e1d1aa6f?q=80&w=1000", new DateTime(2026, 6, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 3, "Private ceremony.", new DateTime(2026, 7, 20, 22, 0, 0, 0, DateTimeKind.Unspecified), "Smith Wedding", 1, "https://images.unsplash.com/photo-1519741497674-611481863552?q=80&w=1000", new DateTime(2026, 7, 20, 14, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingDate", "CustomerName", "CustomerPhone", "CustomerSurname", "EventId", "VenueId" },
                values: new object[] { 1, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "0123456789", "Tester", 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventId",
                table: "Bookings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VenueId",
                table: "Bookings",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                table: "Events",
                column: "EventTypeId");

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
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
