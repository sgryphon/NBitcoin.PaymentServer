CREATE TABLE dbo.Gateways
	(
	Id uniqueidentifier NOT NULL,
	Created datetimeoffset(7) NOT NULL,
	GatewayNumber int NOT NULL IDENTITY (1, 1),
	Name nvarchar(200) NOT NULL,
	ExtPubKey nvarchar(200) NOT NULL,
	IsActive bit NOT NULL
	)
GO

ALTER TABLE dbo.Gateways ADD CONSTRAINT
	DF_Gateways_Created DEFAULT SYSUTCDATETIME() FOR Created
GO

ALTER TABLE dbo.Gateways ADD CONSTRAINT
	DF_Gateways_IsActive DEFAULT 1 FOR IsActive
GO

ALTER TABLE dbo.Gateways ADD CONSTRAINT
	PK_Gateways PRIMARY KEY NONCLUSTERED 
	(
	Id
	)
GO

CREATE UNIQUE CLUSTERED INDEX IX_Gateways_GatewayNumber ON dbo.Gateways
	(
	GatewayNumber
	)
GO
