using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingDetailsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This creates the actual View in your Azure SQL Database
            migrationBuilder.Sql(@"
        CREATE VIEW vw_BookingDetails AS
        SELECT 
            b.BookingId, 
            b.BookingDate, 
            b.CustomerName, 
            b.CustomerSurname, 
            b.CustomerPhone,
            e.EventName, 
            v.VenueName, 
            v.Location
        FROM Bookings b
        INNER JOIN Events e ON b.EventId = e.EventId
        INNER JOIN Venues v ON b.VenueId = v.VenueId
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vw_BookingDetails");
        }
    }
}
