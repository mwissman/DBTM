IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DBTM')
BEGIN
	exec ('CREATE SCHEMA [DBTM];')
END

GO

--------------------------------------------------------------------------------------------------------------------------------

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'DBTM' AND  TABLE_NAME = 'History'))
BEGIN

	CREATE TABLE [DBTM].[History](
		[VersionHistoryId] [int] IDENTITY(1,1) NOT NULL,
		[VersionNumber] [int] NOT NULL,
		[StatementId] [uniqueidentifier] NOT NULL,
		[RunDate] [datetime] NOT NULL  DEFAULT (getdate()),
		[RunBy] [varchar](50) NOT NULL DEFAULT (suser_name()),
		[DeploymentStep] [varchar](50) NOT NULL CHECK ([DeploymentStep] IN('PreDeployment', 'Backfill', 'PostDeployment')),
		[Action] [varchar](50) NOT NULL CHECK ([Action] IN('Upgrade', 'Rollback'))
	
	 CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED 
	(
		[VersionHistoryId] ASC
	)
	) 

END
GO
--------------------------------------------------------------------------------------------------------------------------------
IF OBJECT_ID('[DBTM].sp_CanRunStatementUpgrade') IS NULL
BEGIN

	exec('
	CREATE PROCEDURE [DBTM].sp_CanRunStatementUpgrade
		@statementId [uniqueidentifier]

	AS 
	BEGIN
	
		IF NOT EXISTS (SELECT * FROM [DBTM].[History] WHERE [StatementId] = @statementId)
		BEGIN
			return 1
		END
	
		DECLARE @lastStatementAction varchar(50);
		SELECT  @lastStatementAction=[Action] FROM (SELECT TOP 1 [Action] FROM [DBTM].[History] WHERE [StatementId] = @statementId ORDER BY [VersionHistoryId] DESC)  as lastStatement;

		IF @lastStatementAction=''Rollback''
		BEGIN
			return 1
		END

		return 0
	END')
END
GO

--------------------

IF OBJECT_ID('[DBTM].sp_RecordStatementExecuted') IS NULL
BEGIN
	exec('
	CREATE PROCEDURE [DBTM].sp_RecordStatementExecuted
		@versionNumber int,
		@statementId [uniqueidentifier],
		@deploymentStep varchar(50),
		@action varchar(50)
	AS 
	BEGIN
		SET NOCOUNT ON
		INSERT INTO [DBTM].[History] ([VersionNumber],[StatementId],[RunDate],[RunBy],[DeploymentStep],[Action])
				VALUES (@versionNumber,@statementId, GETDATE(),suser_name(), @deploymentStep,@action)
		SET NOCOUNT OFF
	END')

END

GO


--------------------


IF OBJECT_ID('[DBTM].sp_version') IS NULL
BEGIN
	exec('
	CREATE PROCEDURE [DBTM].sp_version
	
	AS 
	BEGIN
		SELECT TOP 1 [VersionNumber] FROM [DBTM].[History] WHERE  [Action]<>''Rollback'' ORDER BY [VersionHistoryId] DESC
	END')
END

GO