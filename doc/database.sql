-- ============================================================
--  Project  : PRN222 Lab 02 – Product Management Application
--  Platform : MS SQL Server 2025 (MonsterASP.NET)
--  Schema   : lab2
--  Author   : Database Architect
--  Version  : 1.0.0
-- ============================================================
--
--  DESIGN DECISIONS
--  ─────────────────
--  1. All objects live in the [lab2] schema so multiple lab
--     projects can coexist safely in one MonsterASP.NET database
--     without name collisions.
--  2. Every CREATE block is idempotent: running this script twice
--     is safe. Existing data is never truncated on re-run.
--  3. IDENTITY columns start at 1 / increment by 1 (SQL default).
--  4. String columns use NVARCHAR to support Unicode product names.
--  5. Monetary values use DECIMAL(18,2) – matches the EF Core
--     [Column(TypeName = "decimal(18,2)")] annotation on Product.
--  6. All foreign keys use ON DELETE RESTRICT (NO ACTION) to
--     prevent accidental cascade deletes from the application layer.
--  7. Passwords are plain-text per the Lab MVP assumption stated in
--     the PRD (§ Assumptions). Never do this in production.
--  8. Seed data provides two ready-to-use accounts (one per role),
--     five categories, and eight sample products.
-- ============================================================


-- ============================================================
--  0. PREREQUISITES
-- ============================================================

USE db55627.databaseasp.net;   -- ← Replace with your MonsterASP.NET DB name
GO

-- ============================================================
--  1. SCHEMA
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.schemas WHERE name = N'lab2'
)
BEGIN
    EXEC('CREATE SCHEMA [lab2] AUTHORIZATION [dbo]');
    PRINT '[lab2] schema created.';
END
ELSE
    PRINT '[lab2] schema already exists – skipped.';
GO


-- ============================================================
--  2. TABLES
-- ============================================================

-- ------------------------------------------------------------
--  2.1  Category
--       Must be created before Product (FK target).
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM   sys.tables t
    JOIN   sys.schemas s ON t.schema_id = s.schema_id
    WHERE  s.name = N'lab2' AND t.name = N'Category'
)
BEGIN
    CREATE TABLE [lab2].[Category] (
        [CategoryId]   INT           NOT NULL IDENTITY(1,1),
        [CategoryName] NVARCHAR(100) NOT NULL,

        CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
    );
    PRINT '[lab2].[Category] table created.';
END
ELSE
    PRINT '[lab2].[Category] table already exists – skipped.';
GO


-- ------------------------------------------------------------
--  2.2  Product
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM   sys.tables t
    JOIN   sys.schemas s ON t.schema_id = s.schema_id
    WHERE  s.name = N'lab2' AND t.name = N'Product'
)
BEGIN
    CREATE TABLE [lab2].[Product] (
        [ProductId]     INT              NOT NULL IDENTITY(1,1),
        [ProductName]   NVARCHAR(200)    NOT NULL,
        [UnitPrice]     DECIMAL(18, 2)   NOT NULL,
        [UnitsInStock]  INT              NOT NULL,
        [CategoryId]    INT              NOT NULL,

        CONSTRAINT [PK_Product]             PRIMARY KEY CLUSTERED ([ProductId] ASC),
        CONSTRAINT [FK_Product_Category]    FOREIGN KEY ([CategoryId])
            REFERENCES [lab2].[Category] ([CategoryId])
            ON UPDATE NO ACTION
            ON DELETE NO ACTION,
        CONSTRAINT [CK_Product_UnitPrice]   CHECK ([UnitPrice] >= 0),
        CONSTRAINT [CK_Product_UnitsInStock] CHECK ([UnitsInStock] >= 0)
    );
    PRINT '[lab2].[Product] table created.';
END
ELSE
    PRINT '[lab2].[Product] table already exists – skipped.';
GO


-- ------------------------------------------------------------
--  2.3  AccountMember
--       MemberId is a natural key supplied by the admin
--       (not an IDENTITY) so store managers can set IDs like
--       1001, 1002 for easy employee badge alignment.
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM   sys.tables t
    JOIN   sys.schemas s ON t.schema_id = s.schema_id
    WHERE  s.name = N'lab2' AND t.name = N'AccountMember'
)
BEGIN
    CREATE TABLE [lab2].[AccountMember] (
        [MemberId]       INT           NOT NULL,
        [MemberPassword] NVARCHAR(100) NOT NULL,
        [MemberRole]     TINYINT       NOT NULL,   -- 1 = Manager, 2 = Employee

        CONSTRAINT [PK_AccountMember]          PRIMARY KEY CLUSTERED ([MemberId] ASC),
        CONSTRAINT [CK_AccountMember_Role]     CHECK ([MemberRole] IN (1, 2))
    );
    PRINT '[lab2].[AccountMember] table created.';
END
ELSE
    PRINT '[lab2].[AccountMember] table already exists – skipped.';
GO


-- ============================================================
--  3. INDEXES
--     Clustered PKs are already created above.
--     Add non-clustered indexes on high-frequency lookup columns.
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE  name = N'IX_Product_CategoryId'
      AND  object_id = OBJECT_ID(N'[lab2].[Product]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Product_CategoryId]
        ON [lab2].[Product] ([CategoryId] ASC);
    PRINT 'Index [IX_Product_CategoryId] created.';
END
ELSE
    PRINT 'Index [IX_Product_CategoryId] already exists – skipped.';
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE  name = N'IX_AccountMember_Role'
      AND  object_id = OBJECT_ID(N'[lab2].[AccountMember]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AccountMember_Role]
        ON [lab2].[AccountMember] ([MemberRole] ASC);
    PRINT 'Index [IX_AccountMember_Role] created.';
END
ELSE
    PRINT 'Index [IX_AccountMember_Role] already exists – skipped.';
GO


-- ============================================================
--  4. SEED DATA
--     All inserts are conditional – no duplicates on re-run.
-- ============================================================

-- ------------------------------------------------------------
--  4.1  AccountMember  (2 accounts: 1 Manager, 1 Employee)
-- ------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM [lab2].[AccountMember] WHERE [MemberId] = 1)
    INSERT INTO [lab2].[AccountMember] ([MemberId], [MemberPassword], [MemberRole])
    VALUES (1, 'manager@123', 1);

IF NOT EXISTS (SELECT 1 FROM [lab2].[AccountMember] WHERE [MemberId] = 2)
    INSERT INTO [lab2].[AccountMember] ([MemberId], [MemberPassword], [MemberRole])
    VALUES (2, 'staff@123', 2);

PRINT '2 AccountMember seed rows inserted (skipped if already present).';
GO

-- ------------------------------------------------------------
--  4.2  Category  (5 product categories)
-- ------------------------------------------------------------

SET IDENTITY_INSERT [lab2].[Category] ON;

IF NOT EXISTS (SELECT 1 FROM [lab2].[Category] WHERE [CategoryId] = 1)
    INSERT INTO [lab2].[Category] ([CategoryId], [CategoryName]) VALUES (1, N'Beverages');

IF NOT EXISTS (SELECT 1 FROM [lab2].[Category] WHERE [CategoryId] = 2)
    INSERT INTO [lab2].[Category] ([CategoryId], [CategoryName]) VALUES (2, N'Condiments');

IF NOT EXISTS (SELECT 1 FROM [lab2].[Category] WHERE [CategoryId] = 3)
    INSERT INTO [lab2].[Category] ([CategoryId], [CategoryName]) VALUES (3, N'Confections');

IF NOT EXISTS (SELECT 1 FROM [lab2].[Category] WHERE [CategoryId] = 4)
    INSERT INTO [lab2].[Category] ([CategoryId], [CategoryName]) VALUES (4, N'Dairy Products');

IF NOT EXISTS (SELECT 1 FROM [lab2].[Category] WHERE [CategoryId] = 5)
    INSERT INTO [lab2].[Category] ([CategoryId], [CategoryName]) VALUES (5, N'Produce');

SET IDENTITY_INSERT [lab2].[Category] OFF;

PRINT '5 Category seed rows inserted (skipped if already present).';
GO

-- ------------------------------------------------------------
--  4.3  Product  (8 sample products)
-- ------------------------------------------------------------

SET IDENTITY_INSERT [lab2].[Product] ON;

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 1)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (1, N'Chai', 18.00, 39, 1);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 2)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (2, N'Chang', 19.00, 17, 1);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 3)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (3, N'Aniseed Syrup', 10.00, 13, 2);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 4)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (4, N'Chef Anton''s Cajun Seasoning', 22.00, 53, 2);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 5)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (5, N'Pavlova', 17.45, 29, 3);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 6)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (6, N'Camembert Pierrot', 34.00, 19, 4);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 7)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (7, N'Uncle Bob''s Organic Dried Pears', 30.00, 15, 5);

IF NOT EXISTS (SELECT 1 FROM [lab2].[Product] WHERE [ProductId] = 8)
    INSERT INTO [lab2].[Product] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock], [CategoryId])
    VALUES (8, N'Tofu', 23.25, 35, 5);

SET IDENTITY_INSERT [lab2].[Product] OFF;

PRINT '8 Product seed rows inserted (skipped if already present).';
GO


-- ============================================================
--  5. VERIFICATION QUERIES
--     Run these manually to confirm a healthy schema state.
-- ============================================================

-- Object inventory
SELECT
    s.name              AS [Schema],
    o.name              AS [Object],
    o.type_desc         AS [Type],
    o.create_date       AS [Created],
    o.modify_date       AS [LastModified]
FROM   sys.objects  o
JOIN   sys.schemas  s ON o.schema_id = s.schema_id
WHERE  s.name = N'lab2'
  AND  o.type IN ('U', 'PK', 'F', 'UQ', 'C', 'IX')
ORDER  BY o.type_desc, o.name;

-- Row counts
SELECT 'AccountMember' AS [Table], COUNT(*) AS [Rows] FROM [lab2].[AccountMember]
UNION ALL
SELECT 'Category',                 COUNT(*)              FROM [lab2].[Category]
UNION ALL
SELECT 'Product',                  COUNT(*)              FROM [lab2].[Product];

-- Full product list with category name (mirrors what EF Core will query)
SELECT
    p.[ProductId],
    p.[ProductName],
    p.[UnitPrice],
    p.[UnitsInStock],
    c.[CategoryName]
FROM   [lab2].[Product]  p
JOIN   [lab2].[Category] c ON p.[CategoryId] = c.[CategoryId]
ORDER  BY p.[ProductId];
GO
