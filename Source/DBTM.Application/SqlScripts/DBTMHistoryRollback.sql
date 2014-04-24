--------------------------------------------------------------------------------------------------------------------------------
IF OBJECT_ID('[DBTM].sp_version') IS NOT NULL
BEGIN
	DROP PROCEDURE [DBTM].sp_version
END
GO

--------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('[DBTM].sp_RecordStatementExecuted') IS NOT NULL
BEGIN
	DROP PROCEDURE [DBTM].sp_RecordStatementExecuted
END 

GO

--------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('[DBTM].sp_CanRunStatementUpgrade') IS NOT NULL
BEGIN
	DROP PROCEDURE DBTM.sp_CanRunStatementUpgrade;
END 

GO

--------------------------------------------------------------------------------------------------------------------------------
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'DBTM' AND  TABLE_NAME = 'History'))
BEGIN

	DROP TABLE [DBTM].[History];

END

GO

--------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DBTM')
BEGIN
	exec ('DROP SCHEMA [DBTM];')
END

GO
