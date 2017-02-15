CREATE TABLE dbo.PaymentRequests
	(
	PaymentId uniqueidentifier NOT NULL,
	Created datetimeoffset(7) NOT NULL,
	GatewayId uniqueidentifier NOT NULL,
	Amount decimal(18, 8) NOT NULL,
	Currency nvarchar(4) NULL,
	Reference nvarchar(254) NULL,
	Memo nvarchar(MAX) NULL
	)
GO

ALTER TABLE dbo.PaymentRequests ADD CONSTRAINT
	DF_PaymentRequests_Created DEFAULT SYSUTCDATETIME() FOR Created
GO

ALTER TABLE dbo.PaymentRequests ADD CONSTRAINT
	PK_PaymentRequests PRIMARY KEY NONCLUSTERED 
	(
		PaymentId
	)
GO

CREATE INDEX IX_PaymentRequests_Reference ON dbo.PaymentRequests
	(
		Reference
	)
GO

ALTER TABLE dbo.PaymentRequests ADD CONSTRAINT 
	FK_PaymentRequests_Gateways FOREIGN KEY 
	(
		GatewayId
	)
	REFERENCES dbo.Gateways
	(
		Id
	)
GO
