USE [HeartDisease]
GO
/****** Object:  StoredProcedure [dbo].[SP_ReorganizeIndex]    Script Date: 01-06-2024 10:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_ReorganizeIndex]
    @IndexName NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL NVARCHAR(MAX);

    SELECT 
        @SQL = 'ALTER INDEX ' + QUOTENAME(@IndexName) + ' ON ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(OBJECT_NAME(i.object_id)) + ' REORGANIZE'
    FROM 
        sys.indexes i
    INNER JOIN 
        sys.tables t ON i.object_id = t.object_id
    WHERE 
        i.name = @IndexName;

    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
    END
    ELSE
    BEGIN
        RAISERROR('Index not found: %s', 16, 1, @IndexName);
    END
END
