# ✨ EventEase

**EventEase** is a robust, full-stack ASP.NET Core MVC web application designed to manage whimsical event venues, schedule exciting events, and process bookings. It features a custom "ticket-style" UI, secure role-based access control, and enterprise-grade image handling via Azure.

---

## 🚀 Features

### 🏢 Venue Management
- **Comprehensive CRUD:** Browse, create, edit, and delete event spaces.  
- **Capacity Tracking:** Monitor venue limits to ensure events stay within safety regulations.  
- **Image Gallery:** Visual representation of venues with support for both local uploads and external links.  

### 📅 Event Scheduling & Categorization
- **Dynamic Lookup Tables:** Events are organized by types (e.g., Gala, Workshop, Concert) using a relational database schema for better filtering.  
- **Conflict Resolution:** Built-in logic prevents "Double Bookings" by checking venue availability in real-time before saving a new or edited event.  
- **Ticket UI:** A specialized "ticket-style" details view providing a unique, high-contrast user experience.  

### 🎟️ Master Booking Portal
- **Staff Oversight:** A central hub for staff to manage reservations and track attendance across the platform.  
- **Data Integrity:** Safety constraints prevent the deletion of events that have active ticket bookings to protect customer data.  

### 🔍 Smart Search & Filtering
- **Multi-Parameter Search:** Filter events by Name, Date Range, or Category.  
- **ID Lookup:** Quick search functionality for specific event IDs (e.g., searching `#EE-101`).  

### ☁️ Cloud Integration & Security
- **Azure Blob Storage:** High-availability image hosting using a repository-style `IBlobService`.  
- **Secure Auth:** Custom role-based access control (Admin vs. Specialist) with `PasswordHasher` for encrypted staff credentials.  
- **JS Live Previews:** Real-time image previews during the creation process for immediate visual feedback.  

---

## 🛠️ Technologies Used
- **Framework:** ASP.NET Core MVC (.NET 8)  
- **Language:** C#  
- **Database:** SQL Server (LocalDB for development / Azure SQL for production)  
- **ORM:** Entity Framework Core (Code-First approach)  
- **Cloud Hosting:** Azure App Service & Azure SQL Database  
- **Storage:** Azure Blob Storage  
- **Frontend:** Bootstrap 5, Custom CSS3, Vanilla JavaScript  

---

## 📋 Prerequisites
To run this project locally, you will need:  
- Visual Studio 2022 (with the ASP.NET and web development workload)  
- SQL Server Express (LocalDB)  
- .NET 8 SDK installed  

---

## ⚙️ Getting Started (Local Development)
1. **Clone the repository** and open `EventEase.sln` in Visual Studio.  

2. **Configure the Database Connection:**  
   Open `appsettings.json` and ensure the `DefaultConnection` is pointing to your local SQL Express instance.  
   > Note: The Azure SQL connection string is also provided but should be commented out for local dev to save cloud credits.

3. **Set Up Azure Storage:**  
   Provide your Azure Storage connection string in `appsettings.json` or ensure the `IBlobService` is configured for your environment.

4. **Run Entity Framework Migrations:**  
   Open the Package Manager Console (Tools > NuGet Package Manager) and run:  
   ```powershell
   Update-Database
   
5. **Launch the Application:**
   Press F5 to start the web server and launch the application in your browser.
