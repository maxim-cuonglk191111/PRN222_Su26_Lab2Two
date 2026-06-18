# PRN222 Lab 2: Product Management Razor Pages & SignalR

This project is an advanced iteration of the Product Management System. It replaces traditional MVC Controllers with Razor Pages and integrates SignalR WebSockets to provide real-time UI updates across all active client sessions.

## Architecture

This lab retains the exact same optimized asynchronous backend logic as Lab 1, establishing a "Universal Code" standard:
- **BusinessObjects:** Contains Entity Framework Core Models and the `DbContext`.
- **DataAccessObjects (DAO):** Handles all asynchronous direct database operations.
- **Repositories:** Interfaces and implementations mapping to DAOs.
- **Services:** Business logic layer that orchestrates Repositories.
- **ProductManagementRazorPages:** The presentation layer utilizing ASP.NET Core Razor Pages (`.cshtml.cs` PageModels) and SignalR Hubs.

## Real-Time Synchronization
When an Administrator adds, edits, or deletes a product, the SignalR Hub broadcasts a `LoadAllItems` event to every connected client. The clients' browsers listen to this via JavaScript and automatically refresh the product grid—guaranteeing 100% data consistency for all staff members simultaneously.

## Database Configuration

This project expects an SQL Server instance. 
To safely share a single free-tier database (e.g., MonsterASP.NET) alongside Lab 1, all objects in this project are deployed to the isolated `[lab2]` schema.

### Setup Instructions
1. Run the database setup script located at `doc/db_lab02.sql` on your SQL Server instance. This will safely construct the `[lab2]` schema and seed the initial data.
2. Configure your connection string in `ProductManagementRazorPages/appsettings.json`.
3. Build and run the project:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project ProductManagementRazorPages
   ```

## Cloud Deployment (Render & MonsterASP)

This project is fully containerized via Docker and optimized for free-tier cloud deployment.

### 1. Database Setup (MonsterASP.NET)
1. Go to **MonsterASP.NET** and create a free MS SQL Server database.
2. Connect to your remote database using SSMS (SQL Server Management Studio).
3. Execute the `doc/db_lab02.sql` script provided in the `doc` folder.
   - *This automatically isolates your tables inside the `[lab2]` schema, allowing multiple projects to share the single 1GB free tier without data collisions.*
4. Copy your Database Connection String provided by MonsterASP.

### 2. Web Application Setup (Render.com)
1. Push this Lab 2 Razor Pages repository to GitHub. 
   - *(Note: Ensure the `Dockerfile` is present at the root of the repository being deployed).*
2. Log into **Render.com** and click **New+ -> Web Service**.
3. Connect your GitHub repository.
4. Render will automatically detect the `Dockerfile` and select the **Docker** runtime.
5. Scroll down to the **Environment Variables** section and add:
   - **Key:** `ConnectionStrings__DefaultConnection`
   - **Value:** `Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True;`
   - *Note: You MUST append `TrustServerCertificate=True;` to bypass SSL certificate validation errors between Render's Linux containers and MonsterASP.*

6. Click **Create Web Service**. Render will build the Docker container and deploy your Lab 2 Razor Pages application.
