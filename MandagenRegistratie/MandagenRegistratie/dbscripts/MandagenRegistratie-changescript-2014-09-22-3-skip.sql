GO
USE [master]
GO
ALTER DATABASE MandagenRegistratieDev SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

GO
ALTER DATABASE MandagenRegistratieDev COLLATE SQL_Latin1_General_CP1_CI_AS
GO
ALTER DATABASE MandagenRegistratieDev SET MULTI_USER WITH ROLLBACK IMMEDIATE;
Go

-- DOE DIT HANDMATIG VOOR ALLE KOLOMMEN

SELECT
    col.name, col.collation_name
FROM 
    sys.columns col
WHERE
    object_id = OBJECT_ID('Configuration')

-- ALTER TABLE Configuration
--  ALTER COLUMN ConfigurationValue
--    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
