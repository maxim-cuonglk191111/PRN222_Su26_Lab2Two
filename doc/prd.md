# Product Requirements Document: Lab 02 - Product Management Application (Razor Pages & SignalR)

## Product Overview

**Product Vision:** A streamlined, real-time internal web application that enables store managers and employees to securely and efficiently manage product inventory and categories with instant data synchronization, deployed via a modern containerized cloud architecture.

**Target Users:** Primary users are Store Employees (managing daily stock); Secondary users are Store Managers (overseeing inventory and system access).

**Business Objectives:** * Transition the legacy/local database system to a scalable, distributed cloud environment.

* Eliminate data inconsistency by providing real-time inventory updates across all active user sessions using WebSockets.
* Maximize free-tier infrastructure by decoupling the application: hosting the Web App container on Render and the MS SQL Server on MonsterASP.NET.

**Success Metrics:** * 100% real-time synchronization success rate for active client sessions upon inventory updates.

* Successful automated deployment of the Docker container on Render without exceeding the 500-hour monthly free-tier limit.
* Application maintains stable read/write operations to the external MonsterASP.NET SQL Server under 500ms.

---

## User Personas

### Persona 1: Store Manager (Admin)

* **Demographics:** 35-50 years old, retail management background, moderate technical proficiency.
* **Goals:** Oversee the entire store's inventory, ensure pricing accuracy, and maintain a structured category system.
* **Pain Points:** Outdated inventory lists leading to stockouts; slow, non-responsive local legacy applications.
* **User Journey:** Logs in with elevated privileges (Role 1) -> Views real-time product dashboard -> Adds new product lines -> Adjusts stock units -> Logs out securely.

### Persona 2: Store Employee (Staff)

* **Demographics:** 18-30 years old, daily operations staff, high technical proficiency with web apps.
* **Goals:** Quickly update stock quantities after receiving shipments and look up current product prices.
* **Pain Points:** Not knowing if another employee already updated the stock because the system doesn't refresh automatically.
* **User Journey:** Logs in (Role 2) -> Keeps the Product List page open on a store tablet -> Sees real-time UI updates when stock changes -> Edits specific product quantities -> Saves changes.

---

## Feature Requirements

| Feature | Description | User Stories | Priority | Acceptance Criteria | Dependencies |
| --- | --- | --- | --- | --- | --- |
| **Authentication & Authorization** | Session-based login using MemberId and Password. | As a user, I want to log in securely so that I can access internal store data based on my role. | Must | - Validate credentials against `AccountMember` table.<br>

<br>

<br>- Grant access only to Role 1 or Role 2.<br>

<br>

<br>- Redirect unauthorized users to `/Login`. | External DB connection |
| **Product CRUD Operations** | Interface to Create, Read, Update, and Delete products via Razor Pages. | As an employee, I want to add, edit, and remove products so the inventory is always accurate. | Must | - Forms validate required fields (Name, Price, Stock).<br>

<br>

<br>- Dropdown provided for selecting Categories.<br>

<br>

<br>- Operations directly update the MS SQL DB. | EF Core Setup, Category Data |
| **Real-time Data Synchronization** | Broadcast inventory changes to all connected clients. | As an employee, I want the product list to automatically refresh when someone else makes an update. | Must | - SignalR Hub triggers `LoadAllItems` event on Create/Edit/Delete.<br>

<br>

<br>- Connected clients automatically refresh the table via JavaScript. | SignalR configuration, Render WebSockets |
| **Category Management** | Read operations for category mappings. | As an admin, I want products categorized accurately so they are easy to filter and find. | Should | - `CategoryName` displays alongside `ProductId` in the UI.<br>

<br>

<br>- Categories are fetched directly from the database. | Product CRUD |

---

## User Flows

### Flow 1: Secure Login & Session Initialization

1. User navigates to the application URL on Render (`https://[app-name].onrender.com/Login`).
2. User inputs `MemberId` and `MemberPassword`.
3. System calls `AccountService.GetAccountById()`.
4. System verifies credentials and checks `MemberRole` (Must be 1 or 2).
* **Alternative path:** If role is invalid or user doesn't exist, display "You do not have permission to do this function!"


5. System sets `HttpContext.Session` (HttpOnly, IsEssential).
6. System redirects user to `/Products/Index`.

### Flow 2: Create Product with Real-Time Broadcast

1. Authenticated user navigates to `/Products/Create`.
2. System fetches available Categories and populates a dropdown list.
3. User fills in `ProductName`, `UnitPrice`, `UnitsInStock`, and selects a `Category`.
4. User clicks "Create".
* **Error state:** If form validation fails, reload page with validation error messages.


5. System saves the new Product via `ProductService.SaveProduct()`.
6. Server-side SignalR Hub broadcasts the `LoadAllItems` event to all connected clients.
7. User is redirected to `/Products/Index`. All other active sessions automatically refresh their views.

---

## Non-Functional Requirements

### Performance

* **Load Time:** Initial page load under 2 seconds.
* **Concurrent Users:** Supports up to 50 concurrent active SignalR connections (sufficient for internal store usage).
* **Cold Starts:** Acknowledge that the Render free-tier container may sleep after 15 minutes of inactivity, requiring ~30 seconds to spin up on the next request.

### Security

* **Authentication:** Standard ASP.NET Core Session management with a 20-minute idle timeout.
* **Data Protection:** The MS SQL Server connection string MUST NOT be hardcoded in `appsettings.json`. It must be injected securely at runtime using Render's Environment Variables dashboard.

### Compatibility

* **Devices:** Desktop workstations, POS terminals, and standard tablets.
* **Browsers:** Modern browsers (Chrome 90+, Edge 90+, Firefox 88+, Safari 14+).
* **Screen Sizes:** Responsive design utilizing Bootstrap 5.

---

## Technical Specifications

### File Structure

```text
ProductManagementSolution/
├── Dockerfile                   # CRITICAL: Multi-stage build instructions for Render
├── .dockerignore                # Exclude bin/obj folders from build context
├── BusinessObjects/             # Class Library (.dll)
│   ├── Models/                  # Product, Category, AccountMember
│   └── MyStoreContext.cs        # EF Core DbContext
├── DataAccessObjects/           # Class Library (.dll)
│   ├── ProductDAO.cs
│   ├── CategoryDAO.cs
│   └── AccountDAO.cs
├── Repositories/                # Class Library (.dll)
│   ├── IProductRepository.cs
│   ├── ProductRepository.cs
│   └── ... 
├── Services/                    # Class Library (.dll)
│   ├── IProductService.cs
│   ├── ProductService.cs
│   └── ... 
└── ProductManagementRazorPages/ # ASP.NET Core Web App
    ├── Pages/
    │   ├── Products/            
    │   └── Login.cshtml
    ├── wwwroot/
    │   └── js/
    │       ├── site.js          # SignalR client logic
    │       └── microsoft/signalr/
    ├── Hubs/
    │   └── SignalrServer.cs     # SignalR Hub
    ├── Program.cs               
    └── appsettings.json         

```

### Frontend

* **Technology Stack:** HTML5, CSS3, JavaScript (ES6), ASP.NET Core Razor Pages.
* **Design System:** Bootstrap 5.
* **Real-Time Client:** `@microsoft/signalr` fetched via LibMan.

### Backend

* **Technology Stack:** C#, ASP.NET Core 8.0, SignalR.
* **Architecture:** N-Tier Layered Architecture.

### Database

* **Database:** **MS SQL Server 2025** (Hosted remotely via MonsterASP.NET).
* **ORM:** Entity Framework Core (`Microsoft.EntityFrameworkCore.SqlServer` version 8.x).
* **Design:** The database entity-relationship design and N-Tier architecture are mapped out and maintained using Visual Paradigm to ensure accurate scaffolding.

### Infrastructure & Deployment

* **Containerization:** Linux-based Docker image using official Microsoft .NET 8 SDK (for build) and ASP.NET Core Runtime (for execution).
* **Web Hosting:** Render Web Service (Free Tier).
* **Database Hosting:** MonsterASP.NET MS SQL Database (1GB limit).
* **CI/CD:** Connect Render directly to the GitHub repository. Render will automatically trigger a new Docker build and deployment upon every push to the `main` branch.
* **DNS Configuration:** Custom domain mapping can be configured via external DNS hosting providers by setting up a CNAME record pointing to the `.onrender.com` address.

---

## Analytics & Monitoring

* **Key Metrics:** Number of active SignalR connections, daily active users.
* **Events:** Failed login attempts (Unauthorized access tracking).
* **Dashboards:** Use Render's built-in metrics (Logs, Bandwidth) and MonsterASP.NET's Control Panel for database health.

---

## Release Planning

### MVP (v1.0)

* **Features:** Secure Session Login, N-Tier Architecture, Razor Pages CRUD, EF Core integration with MS SQL Server, Real-time list updates via SignalR, full Docker containerization.
* **Timeline:** 2 Weeks.
* **Success Criteria:** Application successfully deployed via Docker to Render, seamlessly connecting to the MonsterASP database, with SignalR WebSockets firing correctly.

---

## Open Questions & Assumptions

* **Assumption 1:** Render supports WebSockets natively on their free tier, which guarantees that SignalR will function without falling back to long-polling.
* **Assumption 2:** The MonsterASP.NET MS SQL Server allows remote connections from Render's dynamic IP addresses. The connection string must include `TrustServerCertificate=True` to bypass SSL certificate validation errors during the server-to-server handshake.
* **Assumption 3:** Passwords in the `AccountMember` table are stored in plain text strictly for MVP/Lab demonstration purposes.