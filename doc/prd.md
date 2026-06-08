# Product Requirements Document: MyStore Product Management Application

## Product Overview

**Product Vision:** A streamlined, real-time internal web application that enables store managers and employees to securely and efficiently manage product inventory and categories with instant data synchronization.

**Target Users:** Primary users are Store Employees (managing daily stock); Secondary users are Store Managers (overseeing inventory and system access).

**Business Objectives:** * Transition the legacy/local database system to a scalable cloud environment using PostgreSQL.

* Eliminate data inconsistency by providing real-time inventory updates across all active user sessions.
* Secure internal data through role-based access control and session management.

**Success Metrics:** * 100% real-time synchronization success rate for active client sessions upon inventory updates.

* Zero unauthorized data access incidents.
* Application and database deployment successfully operating within cloud free-tier limits without performance degradation.

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

<br>- Grant access only to Role 1 or Role 2.<br>

<br>- Redirect unauthorized/unauthenticated users to the Login page. | Database connection |
| **Product CRUD Operations** | Interface to Create, Read, Update, and Delete products. | As an employee, I want to add, edit, and remove products so the inventory is always accurate. | Must | - Forms validate required fields (Name, Price, Stock).<br>

<br>- Dropdown provided for selecting Categories.<br>

<br>- Successful operations update the PostgreSQL DB. | EF Core Setup, Category Data |
| **Real-time Data Synchronization** | Broadcast inventory changes to all connected clients. | As an employee, I want the product list to automatically refresh when someone else makes an update. | Must | - SignalR Hub triggers `LoadAllItems` event on Create/Edit/Delete.<br>

<br>- Connected clients automatically refresh the product table without manual page reloads. | SignalR configuration |
| **Category Management** | Read operations for category mappings. | As an admin, I want products categorized accurately so they are easy to filter and find. | Should | - `CategoryName` displays alongside `ProductId` in the UI.<br>

<br>- Categories are fetched directly from the database. | Product CRUD |

---

## User Flows

### Flow 1: Secure Login & Session Initialization

1. User navigates to the application URL (`/Login`).
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
* **Response Time:** Database queries via EF Core should resolve in under 500ms.

### Security

* **Authentication:** Standard ASP.NET Core Session management with a 20-minute idle timeout.
* **Authorization:** Role-based checks integrated into PageModel `OnGetAsync` and `OnPostAsync` methods.
* **Data Protection:** Connection strings must be stored securely using environment variables or cloud secrets management, not hardcoded in `appsettings.json` for production.

### Compatibility

* **Devices:** Desktop workstations, POS terminals, and standard tablets.
* **Browsers:** Modern browsers (Chrome 90+, Edge 90+, Firefox 88+, Safari 14+).
* **Screen Sizes:** Responsive design utilizing Bootstrap (built-in with ASP.NET Core Razor Pages templates).

### Accessibility

* **Compliance Level:** WCAG 2.1 AA (baseline).
* **Specific Requirements:** Proper ARIA labels for form inputs, semantic HTML for tables, and clear visual validation error states.

---

## Technical Specifications

### File Structure

```text
ProductManagementSolution/
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
│   └── ... (Category & Account)
├── Services/                    # Class Library (.dll)
│   ├── IProductService.cs
│   ├── ProductService.cs
│   └── ... (Category & Account)
└── ProductManagementRazorPages/ # ASP.NET Core Web App
    ├── Pages/
    │   ├── Products/            # Create, Index, Edit, Details, Delete
    │   ├── Login.cshtml
    │   └── Shared/
    ├── wwwroot/
    │   ├── js/
    │   │   ├── site.js          # SignalR client logic
    │   │   └── microsoft/signalr/
    ├── Hubs/
    │   └── SignalrServer.cs     # SignalR Hub
    ├── Program.cs               # DI, Pipeline, Session & SignalR Config
    └── appsettings.json         # PostgreSQL Connection String

```

### Frontend

* **Technology Stack:** HTML5, CSS3, JavaScript (ES6), ASP.NET Core Razor Pages.
* **Design System:** Bootstrap 5 (Standard template integration).
* **Responsive Design:** Required for standard table displays; forms must utilize mobile-friendly input types.
* **Real-Time Client:** `@microsoft/signalr` fetched via LibMan (unpkg).

### Backend

* **Technology Stack:** C#, ASP.NET Core 8.0, SignalR.
* **Architecture:** N-Tier Layered Architecture (Presentation -> Service -> Repository -> DAO).
* **Dependency Injection:** Scoped lifecycles for Services and Repositories.

### Database

* **Database:** **PostgreSQL** (Migrating from SQL Server).
* **ORM:** Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL` version 8.x instead of `Microsoft.EntityFrameworkCore.SqlServer`).
* **Structure:** - `AccountMember` (MemberId, MemberPassword, FullName, EmailAddress, MemberRole)
* `Categories` (CategoryId, CategoryName)
* `Products` (ProductId, ProductName, CategoryId, UnitsInStock, UnitPrice)



### Infrastructure

* **Hosting:** Cloud Platform (e.g., Render, Heroku, or AWS Elastic Beanstalk).
* **Database Hosting:** Managed PostgreSQL provider with a generous free tier (e.g., Supabase, Neon.tech, or ElephantSQL).
* **CI/CD:** GitHub Actions configured to build the .NET solution and deploy to the selected cloud hosting provider.

---

## Analytics & Monitoring

* **Key Metrics:** Number of active SignalR connections, daily active users, CRUD operation error rates.
* **Events:** Failed login attempts (Unauthorized access tracking).
* **Dashboards:** Built-in cloud provider metrics (CPU, RAM, DB Connections).
* **Alerting:** Alert if PostgreSQL database connection pool reaches 80% capacity (crucial for free-tier DBs).

---

## Release Planning

### MVP (v1.0)

* **Features:** Secure Session Login, N-Tier Architecture, CRUD operations for Products via Razor Pages, Entity Framework Core integration with PostgreSQL, Real-time list updates via SignalR.
* **Timeline:** 2 Weeks.
* **Success Criteria:** Application successfully deployed to the cloud, reading and writing to the cloud PostgreSQL database, with SignalR events firing correctly across multiple devices.

### Future Releases

* **v1.1:** Add Category management (CRUD for categories). Implement password hashing (e.g., BCrypt) replacing plain-text passwords.
* **v1.2:** Add pagination, filtering, and search functionality to the Product Index page to handle larger datasets.
* **v2.0:** Implement JWT-based authentication to support a potential mobile app frontend alongside the Web App.

---

## Open Questions & Assumptions

* **Question 1:** Since we are moving to PostgreSQL, does the existing legacy `MyStore` database need to be migrated via a script, or will we start with a fresh seed data script using EF Core Migrations? - **The existing database schema is preserved, and a migration script will be generated to update the schema in the new PostgreSQL database.**
* **Question 2:** Will the free tier of the chosen cloud provider support WebSockets natively without required workarounds for SignalR to function optimally? - **The free tier of the chosen cloud provider supports WebSockets natively, and no workarounds are required for SignalR to function optimally.**
* **Assumption 1:** The `appsettings.json` provided in the lab uses `TrustServerCertificate=True` for local SQL server. We assume a proper SSL/TLS connection string will be provided for the cloud PostgreSQL instance. - **The `appsettings.json` file will be updated to use a proper SSL/TLS connection string for the cloud PostgreSQL instance, and `TrustServerCertificate=True` will be removed.**
* **Assumption 2:** Passwords in the lab's `AccountMember` table are stored in plain text. It is assumed this is strictly for MVP/Lab purposes and will be addressed in v1.1. - **The passwords in the AccountMember table will be hashed using BCrypt in v1.1.**

---

## Appendix

### Competitive Analysis

* **Competitor 1 (Off-the-shelf POS Systems):** Strengths: Feature-rich. Weaknesses: High subscription costs, closed ecosystem.
* **Competitor 2 (Excel/Google Sheets):** Strengths: Free, highly customizable. Weaknesses: Lacks strict data validation, poor concurrent user handling, no real-time push events for UI.

### User Research Findings

* **Finding 1:** Store employees often overlap their duties; if two employees update stock simultaneously on a static page, the last save overwrites accurate data. Real-time updates (SignalR) are a critical mitigation.
* **Finding 2:** Cloud deployment is necessary as the store plans to allow managers to check stock levels remotely, which the local SQL Server setup prevented.

### AI Conversation Insights

* **AI-Suggested Improvements:** The shift from SQL Server to PostgreSQL requires changing the EF Core NuGet package to `Npgsql.EntityFrameworkCore.PostgreSQL`. The scaffolding command (`dotnet ef dbcontext scaffold`) will need to be updated to use the PostgreSQL connection string format and the Npgsql provider.
* **AI-Generated Edge Cases:** Session timeouts mid-operation. If a user spends 25 minutes filling out the "Create Product" form and hits submit, the session will have expired (20 min limit). The application must handle this gracefully and redirect to login without crashing.

### Glossary

* **SignalR:** A software library for ASP.NET developers that simplifies the process of adding real-time web functionality to applications.
* **N-Tier Architecture:** A software architecture pattern where the application is separated into logical layers (Presentation, Business Logic, Data Access) to improve maintainability.
* **EF Core (Entity Framework Core):** An open-source object-relational mapping (ORM) framework for ADO.NET, allowing developers to work with a database using .NET objects.