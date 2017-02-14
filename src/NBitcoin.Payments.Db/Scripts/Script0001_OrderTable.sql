CREATE TABLE dbo.Orders
	(
	Id uniqueidentifier NOT NULL,
	Created datetimeoffset(7) NOT NULL,
	OrderNumber int NOT NULL IDENTITY (2000, 1),
	Name nvarchar(100) NOT NULL,
	Email nvarchar(254) NULL,
	[Description] nvarchar(MAX) NULL,
	Amount decimal(18, 3) NOT NULL,
	Currency nvarchar(4) NOT NULL
	)
GO

ALTER TABLE dbo.Orders ADD CONSTRAINT
	DF_Orders_Created DEFAULT SYSUTCDATETIME() FOR Created
GO

ALTER TABLE dbo.Orders ADD CONSTRAINT
	PK_Orders PRIMARY KEY NONCLUSTERED 
	(
	Id
	)
GO

CREATE UNIQUE CLUSTERED INDEX IX_Orders_OrderNumber ON dbo.Orders
	(
	OrderNumber
	)
GO
