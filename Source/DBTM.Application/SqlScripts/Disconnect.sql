USE [Master]
IF EXISTS (SELECT * FROM master.dbo.sysdatabases WHERE name = '[DATABASE_NAME]')
	BEGIN
		SET NOCOUNT ON
		DECLARE @thespid AS int
		DECLARE @name AS varchar(20)
		DECLARE @myquery AS varchar (200)
		DECLARE @query varchar(1000)
		DECLARE @thedbid AS varchar(100)

		-- set dbname
		SELECT @thedbid = [dbid] FROM master..sysdatabases WHERE [name] = '[DATABASE_NAME]'
		SET @myquery = 'select req_spid FROM master..syslockinfo WHERE rsc_dbid = ' + @thedbid
		CREATE TABLE #spidnames (id int identity(1,1) ,name varchar(200))
		INSERT INTO #spidnames(name) EXEC (@myquery)
		DECLARE mycursor CURSOR FOR SELECT name FROM #spidnames
		OPEN mycursor
		FETCH NEXT FROM mycursor INTO @name WHILE (@@fetch_status =0)
		BEGIN
			SET @query = 'kill '+ @name
			EXEC (@query)
			FETCH NEXT FROM mycursor INTO @name
		END
		DROP TABLE #spidnames
		CLOSE mycursor
		DEALLOCATE mycursor
		SET NOCOUNT OFF
	END