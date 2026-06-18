# Product Requirements Document: Lab 02 - Product Management Application (ASP.NET Core Razor Pages)

## Product Overview

**Product Vision:** An enterprise-grade, lightweight Product Management System engineered to offer store operators a clean web interface for tracking inventory and categorizing items securely. It replaces legacy desktop frameworks with a modern, cloud-optimized ASP.NET Core Razor Pages multi-tier architectural pattern.

**Target Users:** * **Store Managers (Administrators):** Need comprehensive control over full catalog structures, inventory volumes, and price adjustments.

* **Store Staff (Regular Members):** Need quick read access to check stock status and update basic details without system-wide administrative access.

**Business Objectives:** * Migrate legacy desktop systems into a responsive cloud-native web format using the Razor Pages & SignalR pattern.

* Maximize deployment cost efficiency by leveraging MonsterASP.NET's 1GB MS SQL Server free tier, ensuring 100% compatibility with Entity Framework Core SQL Server providers.
* Guarantee strict system protection via decoupled tier boundaries and robust session isolation.
* Containerize the web application using Docker to enable seamless free-tier deployment on cloud platforms like Render.

**Success Metrics:** * **Operational Cost Reduction:** Zero database maintenance costs by effectively utilizing cloud free-tier computing.

* **Data Integrity:** Zero orphaned products through strictly enforced relational foreign-key integrity constraints within SQL Server.
* **System Responsiveness:** Core page interactions and catalog updates completing within 200ms.
* **Deployment Success:** Docker container builds and runs successfully on Render without exceeding memory limits.

---

## User Personas

### Persona 1: Admin Alex

* **Demographics:** 34 years old, Store Inventory Manager, high technical proficiency in data administration.
* **Goals:** Maintain category hierarchies, modify stock units, delete obsolete entries, and control access permissions.
* **Pain Points:** Hard locked into a desktop workspace, unable to access operations from home, and limited by data thresholds on legacy configurations.
* **User Journey:** Logs in securely, explores the system dashboard, reviews critical low-stock alerts, adjusts unit configurations, and adds new store categories.

### Persona 2: Staff Sam

* **Demographics:** 22 years old, Front-counter Associate, baseline technical proficiency.
* **Goals:** Swiftly query product details, check units in stock for customers, and modify prices during store promotions.
* **Pain Points:** Overwhelmed by cluttered database structures and accidentally changing structural data.
* **User Journey:** Authenticates through a web window, queries a targeted keyword phrase, inspects stock depths, updates individual field entries, and logs out safely.

---

## Feature Requirements

| Feature | Description | User Stories | Priority | Acceptance Criteria | Dependencies |
| --- | --- | --- | --- | --- | --- |
| **Authentication System** | Secure email and password entry with active state tracking. | As a registered member, I want to log in using my email and password so that I can access inventory screens. | **Must** | - Redirect anonymous paths to `/Account/Login`. <br>

<br>

<br>- Session automatically times out after 20 minutes of inactivity. <br>

<br>

<br>- Password entries masked in the UI. | `AccountRepository`, `Session Middleware` |
| **Category Segmentation** | View and manage distinct structural departments. | As a store manager, I want to build clean classifications so products stay grouped logically. | **Must** | - Enforce unique names. <br>

<br>

<br>- Category IDs must link perfectly to the related inventory items. | `CategoryRepository` |
| **Product Lifecycle (CRUD)** | Complete creation, display, editing, and deletion mechanics via Razor Pages PageModels. | As an inventory specialist, I want full management control of stock rows to maintain accurate catalog entries. | **Must** | - Block blank strings on names. <br>

<br>

<br>- Restrict numeric entries to positive values. <br>

<br>

<br>- Show confirmation modal/page prior to removal. | `ProductRepository`, `CategoryService` |
| **Access Control (RBAC)** | Role restrictions bound to specific feature access paths. | As a floor worker, I want restricted viewing boundaries so I do not accidentally clear entire categories. | **Should** | - Hide structural controls from standard roles. <br>

<br>

<br>- Block dangerous REST routing actions unless an executive account key matches. | `Authentication System` |

---

## User Flows

### Flow 1: Secure Authentication & Dashboard Access

1. The user navigates to the core portal landing route (`/` or `/Account/Login`).
2. The middleware detects an unauthenticated session state and triggers an internal route correction to `/Account/Login`.
3. The member enters an email address and a string password, then clicks **Login**.

* *Alternative Path:* If database values do not match the inputs, the application displays an immediate, user-friendly error string: `"Invalid username or password."`
* *Error State:* If the connection to MS SQL Server drops, the engine returns an explicit database unavailable warning block.

4. The system updates the session cache with parameters for `UserId` and `Username`, then transfers routing control to `/Products/Index/`.

### Flow 2: Complete New Product Catalog Insertion

1. An authorized user navigates to `/Products/Create`.
2. The interface uses a clean view lookup loop (via `ViewBag`/`ViewData`) to present an explicit selection dropdown containing every active catalog category option.
3. The administrator fills out fields for product name, units in stock, and unit pricing, then presses **Save**.

* *Alternative Path:* If formatting constraints fail (e.g., negative prices or empty strings), validation alerts fire inline without breaking user form states.

4. The application inserts the entry into MS SQL Server via `ProductsController` and changes the view back to the index grid view layout.

---

## Non-Functional Requirements

### Performance

* **Load Time:** Core product list calculations and UI draws must process within 300ms under standard loads.
* **Concurrent Users:** Architecture safely accommodates standard internal traffic utilizing optimized EF Core DB context pooling.
* **Cold Starts:** When deployed on Render Free Tier, the application may enter sleep mode after 15 minutes of inactivity; initial startup after sleep will take ~30-50 seconds.

### Security

* **Authentication:** Enforced throughout all secure areas via `HttpContext.Session` checks.
* **Authorization:** Int fields handle specific system permissions, separating regular views from administrative configurations.
* **Data Protection:** Connection strings must be isolated outside source control configurations using secure production environment variables on Render.

### Compatibility

* **Devices:** Optimized across all target workstation form factors, desktop viewboxes, and warehouse tablet layouts.
* **Browsers:** Confirmed structural consistency across Chrome, Edge, Safari, and Firefox.
* **Screen Sizes:** Fluid layout support covering responsive transitions from 768px up to 1920px width brackets.

### Accessibility

* **Compliance Level:** Form controls and core layouts adhere closely to general WCAG 2.1 AA accessibility principles.
* **Specific Requirements:** All form text boxes match target context tags, structural tables utilize clear table header declarations, and color changes use strong contrast margins.

---

## Technical Specifications

### File Structure

```text
ProductManagementSolution/
│
├── Dockerfile                   # CRITICAL: Multi-stage Docker build for Render deployment
├── .dockerignore                # Excludes local bin/obj and redundant files from Docker context
│
├── BusinessObjects/
│   ├── Models/
│   │   ├── AccountMember.cs
│   │   ├── Category.cs
│   │   └── Product.cs
│   ├── MyStoreContext.cs
│   └── BusinessObjects.csproj
│
├── DataAccessObjects/
│   ├── AccountDAO.cs
│   ├── CategoryDAO.cs
│   ├── ProductDAO.cs
│   └── DataAccessObjects.csproj
│
├── Repositories/
│   ├── IAccountRepository.cs
│   ├── ICategoryRepository.cs
│   ├── IProductRepository.cs
│   ├── AccountRepository.cs
│   ├── CategoryRepository.cs
│   ├── ProductRepository.cs
│   └── Repositories.csproj
│
├── Services/
│   ├── IAccountService.cs
│   ├── ICategoryService.cs
│   ├── IProductService.cs
│   ├── AccountService.cs
│   ├── CategoryService.cs
│   ├── ProductService.cs
│   └── Services.csproj
│
└── ProductManagementRazor Pages & SignalR/
    ├── PageModels/
    │   ├── AccountController.cs
    │   └── ProductsController.cs
    ├── Views/
    │   ├── Account/
    │   │   └── Login.cshtml
    │   └── Products/
    │       ├── Create.cshtml
    │       ├── Delete.cshtml
    │       ├── Details.cshtml
    │       ├── Edit.cshtml
    │       └── Index.cshtml
    ├── appsettings.json
    ├── Program.csproj
    └── Program.cs

```

### Frontend

* **Technology Stack:** ASP.NET Core Razor Pages utilizing Razor View engine (`.cshtml`).
* **Design System:** Standard Bootstrap 5 setup styled for modern responsive application layouts.
* **Responsive Design:** Dynamic tables that shift naturally into scannable cards on narrow mobile breakpoints.

### Backend

* **Technology Stack:** .NET 8 Framework runtimes.
* **Architecture:** Enforces a clean N-Tier layered repository architecture (Razor Pages & SignalR -> Service -> Repository -> DAO) handling standard transactional web states.
* **Database:** **MS SQL Server 2025**. Tables are structured to map seamlessly to entity definitions via EF Core (`Microsoft.EntityFrameworkCore.SqlServer`):

```sql
-- MS SQL Server (T-SQL) Scaffolding Commands & Compatible DDL Scripts

CREATE TABLE [AccountMember] (
    [MemberID] NVARCHAR(20) PRIMARY KEY,
    [MemberPassword] NVARCHAR(80) NOT NULL,
    [FullName] NVARCHAR(80) NOT NULL,
    [EmailAddress] NVARCHAR(100) NOT NULL,
    [MemberRole] INT NOT NULL
);

CREATE TABLE [Categories] (
    [CategoryID] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoryName] NVARCHAR(15) NOT NULL
);

CREATE TABLE [Products] (
    [ProductID] INT IDENTITY(1,1) PRIMARY KEY,
    [ProductName] NVARCHAR(40) NOT NULL,
    [CategoryID] INT NOT NULL,
    [UnitsInStock] SMALLINT NULL,
    [UnitPrice] MONEY NULL,
    CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryID]) 
        REFERENCES [Categories] ([CategoryID]) ON DELETE CASCADE
);

```

### Infrastructure & Deployment

* **Containerization:** The application is packaged into a Linux-based Docker container. The `Dockerfile` must utilize a multi-stage build: `mcr.microsoft.com/dotnet/sdk:8.0` for building/publishing the `.sln` and `mcr.microsoft.com/dotnet/aspnet:8.0` for the lightweight runtime execution.
* **Web Hosting:** Hosted on Render (Web Service) using the GitHub integration. Render automatically detects the `Dockerfile` at the repository root and builds the image.
* **Database Hosting:** Managed MS SQL Server database cluster tier hosted on MonsterASP.NET (1GB storage limit).
* **CI/CD:** Automated builds compile code assets and push deployment layers via GitHub to Render upon every commit to the main branch.

---

## Analytics & Monitoring

* **Key Metrics:** Monitors authentication error rates, catalog modification speeds, and active concurrent storage connections.
* **Events:** Tracks security anomalies, standard inventory record removals, and system configuration exceptions.
* **Dashboards:** Operational summaries displaying application processing metrics (via Render Logs) and database cluster throughput patterns (via MonsterASP Control Panel).
* **Alerting:** Real-time logging notices capture persistent query execution drops or systemic data connection issues (e.g., reaching the 1GB MonsterASP limit).

---

## Release Planning

### MVP (v1.0)

* **Features:** Complete cross-tier Razor Pages & SignalR architecture supporting session authentication and core transactional CRUD workflows. Full Docker containerization for cloud deployment.
* **Timeline:** 3-week engineering window.
* **Success Criteria:** Zero deployment integration issues; Render successfully builds the Docker container and establishes a connection to the MonsterASP MS SQL Server.

### Future Releases

* **v1.1:** Adds internal export tools to process stock datasets into structured files.
* **v1.2:** Supports configurable low-stock triggers that send automated email notices to supervisors.
* **v2.0:** Integrates real-time barcode scanning capabilities for field-level mobile inventory management.

---

## Open Questions & Assumptions

* **Question 1:** Are password values hashed prior to comparison, or does legacy migration require plaintext comparisons for initial validation cycles?
* *Answer:* Passwords are hashed prior to comparison.


* **Question 2:** Will store managers require deep structural category adjustment capabilities from mobile views, or are operations restricted to desktop environments?
* *Answer:* Store managers require deep structural category adjustment capabilities from mobile views, so operations are not restricted to desktop environments.


* **Assumption 1:** Session values are cached securely within single instance memory pools without requiring isolated distributed cache networks.
* **Assumption 2:** The underlying cloud server infrastructure supports native MS SQL Server connections via the standard `System.Data.SqlClient` or `Microsoft.Data.SqlClient` drivers without additional firewall/proxy restrictions.
* **Assumption 3:** The `Dockerfile` is placed at the root directory of the solution so that it has access to all project folders (`BusinessObjects`, `DataAccessObjects`, `Repositories`, `Services`, and `ProductManagementRazor Pages & SignalR`) during the `dotnet build` stage.

---

## Appendix

### Glossary

* **Razor Pages & SignalR (Model-View-Controller):** A software design pattern that separates internal representations of information from the ways information is presented to and accepted from the user.
* **Multi-Tier Architecture:** An architectural pattern that enforces separate logic layers for data structures, business logic, processing components, and rendering interfaces.
* **ORM (Object-Relational Mapping):** Code components that translate underlying database tables into native development code classes (e.g., Entity Framework Core).
* **RBAC (Role-Based Access Control):** Security enforcement schemas that restrict feature capabilities using profile definitions.
* **Docker:** A set of platform-as-a-service products that use OS-level virtualization to deliver software in packages called containers.