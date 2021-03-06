/*
   maandag 11 augustus 201423:01:52
   User: 
   Server: SONYVAIO\SQLEXPRESS
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
ALTER TABLE dbo.Mandagen
	DROP CONSTRAINT FK_Mandagen_Vakmansoort
GO
ALTER TABLE dbo.Vakmansoort SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Vakmansoort', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Vakmansoort', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Vakmansoort', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Mandagen
	DROP COLUMN VakmansoortId
GO
ALTER TABLE dbo.Mandagen SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'CONTROL') as Contr_Per 