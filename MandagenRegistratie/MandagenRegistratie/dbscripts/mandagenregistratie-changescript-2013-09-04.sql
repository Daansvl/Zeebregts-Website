/*
   woensdag 4 september 201317:15:13
   User: sa
   Server: SQL-SERVER
   Database: MandagenRegistratie
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
ALTER TABLE dbo.Gebruiker ADD
	[Read] bit NOT NULL CONSTRAINT DF_Gebruiker_Lezen DEFAULT 0,
	[Update] bit NOT NULL CONSTRAINT DF_Gebruiker_Updaten DEFAULT 0,
	[Create] bit NOT NULL CONSTRAINT DF_Gebruiker_Create DEFAULT 0,
	Write bit NOT NULL CONSTRAINT DF_Gebruiker_Write DEFAULT 0,
	Admin bit NOT NULL CONSTRAINT DF_Gebruiker_Admin DEFAULT 0
GO
ALTER TABLE dbo.Gebruiker SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Gebruiker', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Gebruiker', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Gebruiker', 'Object', 'CONTROL') as Contr_Per 