# ✨ EventEase 

EventEase is a robust, full-stack ASP.NET Core MVC web application designed to manage whimsical event venues, schedule exciting events, and process bookings. It features a custom "ticket-style" UI, secure role-based access, and dual-mode image handling (file uploads and external URLs).

## 🚀 Features

* **Venue Management:** Browse, create, edit, and delete event spaces. Includes capacity tracking and a beautiful image gallery.
* **Event Scheduling:** Schedule events and assign them to specific venues. Features a live calendar view with real-time status badges.
* **Master Booking List:** A comprehensive portal for staff to manage reservations and track event capacities.
* **Smart Search:** Built-in filtering across Venues (by name/capacity), Events (by name), and Bookings (by ID/Event Name).
* **Dual Image Handling:** Support for both local file uploads (saved to Azure Blob/Local Storage) and external image URLs, complete with live JS previews.
* **Secure Authentication:** Custom Staff login system featuring `PasswordHasher` for secure, encrypted credentials. Role-based access (Admin vs. Specialist) controls what users can see and do.

## 🛠️ Technologies Used

* **Framework:** ASP.NET Core MVC (.NET 8/Current)
* **Language:** C#
* **Database:** SQL Server (LocalDB / Azure SQL)
* **ORM:** Entity Framework Core
* **Frontend:** HTML5, Bootstrap 5, Custom CSS, basic Vanilla JS
* **Cloud Hosting:** Azure App Service & Azure SQL Database

## 📋 Prerequisites

To run this project locally, you will need:
* Visual Studio 2022 (with the ASP.NET and web development workload)
* SQL Server Express (LocalDB)
* .NET SDK installed

## ⚙️ Getting Started (Local Development)

1. **Clone the repository** and open the `EventEase.sln` file in Visual Studio.
2. **Configure the Database Connection:**
   Open `appsettings.json` and ensure the `DefaultConnection` is pointing to your local SQL Express instance. 
   *(Note: The Azure SQL connection string is also provided but should be commented out for local dev to save cloud credits).*
3. **Run Entity Framework Migrations:**
   Open the **Package Manager Console** (Tools > NuGet Package Manager) and run the following command to build the tables and seed the initial data:
   ```powershell
   Update-Database
