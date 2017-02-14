CREATE TABLE dbo.BitcoinPayments
	(
	Id uniqueidentifier NOT NULL,
	Created datetimeoffset(7) NOT NULL,
	IndexNumber int NOT NULL IDENTITY (1000, 1),
	OrderReference nvarchar(50) NOT NULL,
	AmountBtc decimal(18, 8) NOT NULL,
	PaymentAddress nvarchar(50) NULL,
	OriginalCurrency nvarchar(4) NULL,
	ConversionRate decimal(18, 8) NULL,
	ProcessingStatus int NOT NULL,
	ProcessingStatusModified datetimeoffset(7) NOT NULL
	)
GO

ALTER TABLE dbo.BitcoinPayments ADD CONSTRAINT
	DF_BitcoinPayments_Created DEFAULT SYSUTCDATETIME() FOR Created
GO

ALTER TABLE dbo.BitcoinPayments ADD CONSTRAINT
	PK_BitcoinPayments PRIMARY KEY NONCLUSTERED 
	(
	Id
	)
GO

CREATE UNIQUE CLUSTERED INDEX IX_BitcoinPayments_IndexNumber ON dbo.BitcoinPayments
	(
	IndexNumber
	)
GO
