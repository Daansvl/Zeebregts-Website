/*
   donderdag 25 juli 201320:36:42
   User: sa
   Server: SQL-SERVER
   Database: MandagenRegistratieBeta
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Setting
	(
	SettingsId int NOT NULL IDENTITY (1, 1),
	GebruikerId int NOT NULL,
	SettingsNaam nvarchar(50) NOT NULL,
	SettingsValue nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Setting SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Setting ON
GO
IF EXISTS(SELECT * FROM dbo.Setting)
	 EXEC('INSERT INTO dbo.Tmp_Setting (SettingsId, GebruikerId, SettingsNaam, SettingsValue)
		SELECT SettingsId, GebruikerId, SettingsNaam, CONVERT(nvarchar(MAX), SettingsValue) FROM dbo.Setting WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Setting OFF
GO
DROP TABLE dbo.Setting
GO
EXECUTE sp_rename N'dbo.Tmp_Setting', N'Setting', 'OBJECT' 
GO
ALTER TABLE dbo.Setting ADD CONSTRAINT
	PK_Setting PRIMARY KEY CLUSTERED 
	(
	SettingsId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Setting', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Setting', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Setting', 'Object', 'CONTROL') as Contr_Per 