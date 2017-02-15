CREATE TABLE dbo.PaymentDetails
	(
	PaymentId uniqueidentifier NOT NULL,
	Created datetimeoffset(7) NOT NULL,
	KeyIndex int NOT NULL,
	PaymentAddress nvarchar(50) NOT NULL,
	AmountBtc decimal(18, 8) NOT NULL,
	OriginalCurrency nvarchar(4) NULL,
	ConversionRate decimal(18, 8) NULL,
	ProcessingStatus int NOT NULL,
	ProcessingStatusModified datetimeoffset(7) NOT NULL
	)
GO

ALTER TABLE dbo.PaymentDetails ADD CONSTRAINT
	DF_Payments_Created DEFAULT SYSUTCDATETIME() FOR Created
GO

ALTER TABLE dbo.PaymentDetails ADD CONSTRAINT
	PK_PaymentDetails PRIMARY KEY NONCLUSTERED 
	(
		PaymentId
	)
GO

ALTER TABLE dbo.PaymentDetails ADD CONSTRAINT 
	FK_PaymentDetails_PaymentRequests FOREIGN KEY 
	(
		PaymentId
	)
	REFERENCES dbo.PaymentRequests
	(
		PaymentId
	)
GO

CREATE UNIQUE CLUSTERED INDEX IX_BitcoinPayments_PaymentAddress ON dbo.BitcoinPayments
	(
	PaymentAddress
	)
GO
