/*
   dinsdag 12 augustus 20141:04:19
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
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT FK_Vakman_Bedrijf
GO
ALTER TABLE dbo.Bedrijf SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Bedrijf', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Bedrijf', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Bedrijf', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Vakman SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'CONTROL') as Contr_Per 