-- ============================================================
-- Production T-SQL Setup Script
-- Project  : Product Management System  (PRN222 Su26 – Lab 1)
-- Schema   : store
-- Platform : MonsterASP.NET  –  SQL Server 2025
-- ============================================================
-- Design notes
--   • All objects live in the [lab2] schema so multiple projects
--     can coexist in a single MonsterASP.NET 1 GB database without
--     name collisions or DROP/CREATE side-effects on other projects.
--   • Every statement is idempotent: safe to run repeatedly.
--   • Constraint names follow the convention:
--       PK_  – primary key
--       UQ_  – unique constraint
--       FK_  – foreign key
--       CHK_ – check constraint
--       IX_  – non-clustered index
--   • Password hashing matches the .NET implementation exactly:
--       SHA256.HashData(Encoding.UTF8.GetBytes(password))  →  lowercase hex
--     For ASCII-only passwords HASHBYTES('SHA2_256', VARCHAR) produces
--     the same byte sequence as UTF-8, so the SQL hash below is safe
--     for seeding.  Non-ASCII passwords must be hashed by the app.
-- ============================================================


-- ============================================================
-- SECTION 1  –  SCHEMA
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = N'lab2')
    EXEC(N'CREATE SCHEMA [lab2] AUTHORIZATION [dbo]');
GO


-- ============================================================
-- SECTION 2  –  TABLES
-- ============================================================

-- ----------------------------------------------------------
-- 2.1  AccountMember
--      Natural key: MemberID (staff badge / employee code).
--      EmailAddress carries a unique constraint – the app
--      authenticates by email, not by MemberID.
--      MemberRole domain: 1 = Admin, 2 = Staff.
-- ----------------------------------------------------------
IF OBJECT_ID(N'[lab2].[AccountMember]', N'U') IS NULL
CREATE TABLE [lab2].[AccountMember]
(
    [MemberID]       NVARCHAR(20)   NOT NULL,
    [MemberPassword] NVARCHAR(80)   NOT NULL,   -- SHA-256 hex, 64 chars
    [FullName]       NVARCHAR(80)   NOT NULL,
    [EmailAddress]   NVARCHAR(100)  NOT NULL,
    [MemberRole]     INT            NOT NULL,

    CONSTRAINT [PK_AccountMember]
        PRIMARY KEY CLUSTERED ([MemberID]),

    CONSTRAINT [UQ_AccountMember_Email]
        UNIQUE ([EmailAddress]),

    CONSTRAINT [CHK_AccountMember_Role]
        CHECK ([MemberRole] IN (1, 2))          -- 1 = Admin  |  2 = Staff
);
GO


-- ----------------------------------------------------------
-- 2.2  Categories
--      Short taxonomy labels (max 15 chars per domain model).
--      Unique constraint on CategoryName prevents duplicates.
-- ----------------------------------------------------------
IF OBJECT_ID(N'[lab2].[Categories]', N'U') IS NULL
CREATE TABLE [lab2].[Categories]
(
    [CategoryID]   INT          IDENTITY(1,1)  NOT NULL,
    [CategoryName] NVARCHAR(15)                NOT NULL,

    CONSTRAINT [PK_Categories]
        PRIMARY KEY CLUSTERED ([CategoryID]),

    CONSTRAINT [UQ_Categories_Name]
        UNIQUE ([CategoryName])
);
GO


-- ----------------------------------------------------------
-- 2.3  Products
--      Business rules enforced at the DB layer:
--        • UnitsInStock  ≥ 0 (NULL = unknown, not zero)
--        • UnitPrice     > 0 (NULL = unlisted, not free)
--      ON DELETE CASCADE propagates category removal to products,
--      preventing orphaned rows (PRD data-integrity requirement).
-- ----------------------------------------------------------
IF OBJECT_ID(N'[lab2].[Products]', N'U') IS NULL
CREATE TABLE [lab2].[Products]
(
    [ProductID]    INT          IDENTITY(1,1)  NOT NULL,
    [ProductName]  NVARCHAR(40)                NOT NULL,
    [CategoryID]   INT                         NOT NULL,
    [UnitsInStock] SMALLINT                    NULL,
    [UnitPrice]    MONEY                       NULL,

    CONSTRAINT [PK_Products]
        PRIMARY KEY CLUSTERED ([ProductID]),

    CONSTRAINT [FK_Products_Categories]
        FOREIGN KEY ([CategoryID])
        REFERENCES [lab2].[Categories] ([CategoryID])
        ON DELETE CASCADE,

    CONSTRAINT [CHK_Products_UnitsInStock]
        CHECK ([UnitsInStock] IS NULL OR [UnitsInStock] >= 0),

    CONSTRAINT [CHK_Products_UnitPrice]
        CHECK ([UnitPrice] IS NULL OR [UnitPrice] > 0)
);
GO


-- ============================================================
-- SECTION 3  –  NON-CLUSTERED INDEXES
-- ============================================================

-- Authentication lookup: WHERE EmailAddress = @email
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_AccountMember_Email'
      AND [object_id] = OBJECT_ID(N'[lab2].[AccountMember]')
)
    CREATE NONCLUSTERED INDEX [IX_AccountMember_Email]
        ON [lab2].[AccountMember] ([EmailAddress]);
GO

-- Products list + JOIN: avoids key lookup for common columns
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_Products_CategoryID'
      AND [object_id] = OBJECT_ID(N'[lab2].[Products]')
)
    CREATE NONCLUSTERED INDEX [IX_Products_CategoryID]
        ON [lab2].[Products] ([CategoryID])
        INCLUDE ([ProductName], [UnitsInStock], [UnitPrice]);
GO

-- Keyword search: WHERE ProductName LIKE '%keyword%'
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_Products_ProductName'
      AND [object_id] = OBJECT_ID(N'[lab2].[Products]')
)
    CREATE NONCLUSTERED INDEX [IX_Products_ProductName]
        ON [lab2].[Products] ([ProductName]);
GO


-- ============================================================
-- SECTION 4  –  SEED DATA
-- ============================================================
-- Passwords: SHA-256(UTF-8 bytes) → lowercase hex string (64 chars)
--
--   Admin@123  →  computed inline via HASHBYTES (ASCII-safe, matches .NET)
--   Staff@123  →  same
--
-- To hash a new password outside the app:
--   SELECT LOWER(CONVERT(NVARCHAR(64),
--                HASHBYTES('SHA2_256', CONVERT(VARCHAR(MAX), 'YourPassword')),
--                2));

-- 4.1  Account Members
IF NOT EXISTS (SELECT 1 FROM [lab2].[AccountMember] WHERE [MemberID] = N'ADMIN001')
    INSERT INTO [lab2].[AccountMember]
        ([MemberID], [MemberPassword], [FullName], [EmailAddress], [MemberRole])
    VALUES (
        N'ADMIN001',
        LOWER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(MAX), 'Admin@123')), 2)),
        N'Admin Alex',
        N'admin@mystore.com',
        1
    );

IF NOT EXISTS (SELECT 1 FROM [lab2].[AccountMember] WHERE [MemberID] = N'STAFF001')
    INSERT INTO [lab2].[AccountMember]
        ([MemberID], [MemberPassword], [FullName], [EmailAddress], [MemberRole])
    VALUES (
        N'STAFF001',
        LOWER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(MAX), 'Staff@123')), 2)),
        N'Staff Sam',
        N'staff@mystore.com',
        2
    );
GO

-- 4.2  Categories
DECLARE @cats TABLE ([Name] NVARCHAR(15));
INSERT INTO @cats VALUES
    (N'Beverages'),
    (N'Condiments'),
    (N'Dairy'),
    (N'Grains'),
    (N'Seafood');

INSERT INTO [lab2].[Categories] ([CategoryName])
SELECT c.[Name]
FROM   @cats c
WHERE  NOT EXISTS (
    SELECT 1 FROM [lab2].[Categories] x WHERE x.[CategoryName] = c.[Name]
);
GO

-- 4.3  Products  (CategoryID resolved by name – no hard-coded IDENTITY values)
DECLARE @products TABLE
(
    [ProductName]  NVARCHAR(40),
    [CategoryName] NVARCHAR(15),
    [UnitsInStock] SMALLINT,
    [UnitPrice]    MONEY
);
INSERT INTO @products VALUES
    (N'Chai',              N'Beverages',  39, 18.00),
    (N'Chang',             N'Beverages',  17, 19.00),
    (N'Aniseed Syrup',     N'Condiments', 13, 10.00),
    (N'Chef Anton Sauce',  N'Condiments',  0, 21.35),
    (N'Butter',            N'Dairy',       6,  5.50),
    (N'Camembert Pierrot', N'Dairy',       0, 34.00),
    (N'Filo Mix',          N'Grains',     38,  7.00),
    (N'Gnocchi',           N'Grains',     21, 38.00),
    (N'Ikura',             N'Seafood',    31, 31.00),
    (N'Inlagd Sill',       N'Seafood',   112, 19.00);

INSERT INTO [lab2].[Products] ([ProductName], [CategoryID], [UnitsInStock], [UnitPrice])
SELECT
    p.[ProductName],
    c.[CategoryID],
    p.[UnitsInStock],
    p.[UnitPrice]
FROM   @products p
JOIN   [lab2].[Categories] c ON c.[CategoryName] = p.[CategoryName]
WHERE  NOT EXISTS (
    SELECT 1 FROM [lab2].[Products] x WHERE x.[ProductName] = p.[ProductName]
);
GO


-- ============================================================
-- SECTION 5  –  VERIFICATION
-- ============================================================
SELECT
    [Table]  = N'AccountMember',
    [Rows]   = COUNT(*),
    [Sample] = MAX([FullName])
FROM [lab2].[AccountMember]

UNION ALL

SELECT
    N'Categories',
    COUNT(*),
    MAX([CategoryName])
FROM [lab2].[Categories]

UNION ALL

SELECT
    N'Products',
    COUNT(*),
    MAX([ProductName])
FROM [lab2].[Products];
GO
