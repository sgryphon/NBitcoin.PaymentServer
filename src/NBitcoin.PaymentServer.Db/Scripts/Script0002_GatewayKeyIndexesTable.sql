CREATE TABLE dbo.GatewayKeyIndexes
	(
	GatewayId uniqueidentifier NOT NULL,
	Modified datetimeoffset(7) NOT NULL,
	LastKeyIndex int NOT NULL
	)
GO

ALTER TABLE dbo.GatewayKeyIndexes ADD CONSTRAINT
	DF_GatewayKeyIndexes_Modified DEFAULT SYSUTCDATETIME() FOR Modified
GO

ALTER TABLE dbo.GatewayKeyIndexes ADD CONSTRAINT
	PK_GatewayKeyIndexes PRIMARY KEY CLUSTERED 
	(
	GatewayId
	)
GO

ALTER TABLE dbo.GatewayKeyIndexes ADD CONSTRAINT 
	FK_GatewayKeyIndexes_Gateways FOREIGN KEY 
	(
		GatewayId
	)
	REFERENCES dbo.Gateways
	(
		Id
	)
GO
