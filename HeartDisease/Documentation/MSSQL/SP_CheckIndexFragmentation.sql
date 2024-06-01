USE [HeartDisease]
GO
/****** Object:  StoredProcedure [dbo].[SP_CheckIndexFragmentation]    Script Date: 01-06-2024 10:51:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CheckIndexFragmentation]
    @CheckDate DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.FragmentationHistory (TableName, IndexName, FragmentationPercent, CheckDate)
    SELECT 
        OBJECT_NAME(IPS.object_id) AS TableName,
        SI.name AS IndexName,
        IPS.avg_fragmentation_in_percent AS FragmentationPercent,
        @CheckDate AS CheckDate
    FROM 
        sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') AS IPS
    INNER JOIN 
        sys.indexes AS SI ON IPS.object_id = SI.object_id AND IPS.index_id = SI.index_id
    WHERE 
        IPS.avg_fragmentation_in_percent > 0
    ORDER BY 
        IPS.avg_fragmentation_in_percent DESC;
END
