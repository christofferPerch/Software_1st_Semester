USE [HeartDisease]
GO
/****** Object:  StoredProcedure [dbo].[SP_LatestIndexFragmentation]    Script Date: 01-06-2024 10:52:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_LatestIndexFragmentation]
AS
BEGIN
    SET NOCOUNT ON;

    WITH LatestIndexFragmentation AS (
        SELECT 
            ID,
            TableName,
            IndexName,
            FragmentationPercent,
            CheckDate,
            ROW_NUMBER() OVER (PARTITION BY TableName, IndexName ORDER BY CheckDate DESC) AS RowNum
        FROM 
            dbo.FragmentationHistory
    )
    SELECT 
        ID,
        TableName,
        IndexName,
        FragmentationPercent,
        CheckDate
    FROM 
        LatestIndexFragmentation
    WHERE 
        RowNum = 1
    ORDER BY 
        TableName, IndexName;
END
