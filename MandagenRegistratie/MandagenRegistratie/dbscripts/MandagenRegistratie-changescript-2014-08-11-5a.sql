/*
   dinsdag 12 augustus 20140:44:38
   User: 
   Server: SONYVAIO\SQLEXPRESS
   Database: MandagenRegistratieBeta
   Application: 
*/

/* LET OP:
EERST HANDMATIG DE TABELLEN VakmanPlanning ProjectVakman en Route verwijderen

EN ProjectPlanning en ProjectProjectLeider mogen ook verwijderd worden handmatig

*/

/* ACHTERAF HANDMATIG VAKMAN verwijderen en TMP_Vakman RENAMEN naar Vakman 
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
IF (OBJECT_ID('FK_Mandagen_Vakman', 'F') IS NOT NULL)
BEGIN

ALTER TABLE dbo.Mandagen
	DROP CONSTRAINT FK_Mandagen_Vakman
	END
GO

ALTER TABLE dbo.Vakman SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Mandagen SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'CONTROL') as Contr_Per 


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
--ALTER TABLE dbo.Vakman
--	DROP CONSTRAINT FK_Vakman_Bedrijf
--GO
ALTER TABLE dbo.Bedrijf SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Bedrijf', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Bedrijf', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Bedrijf', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Persoon_Loonkosten
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Persoon_Actief
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurMa
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurDi
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurWo
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurDo
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurVr
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurZa
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsChauffeurZo
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijder
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderMa
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderDi
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderWo
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderDo
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderVr
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderZa
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_IsBijrijderZo
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Persoon_Eigenvervoer
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_DefaultBeginuur
GO
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT DF_Vakman_DefaultBeginminuut
GO
CREATE TABLE dbo.Tmp_Vakman
	(
	VakmanId int NOT NULL IDENTITY (1, 1),
	ContactIdOrigineel int NOT NULL,
	BedrijfId int NULL,
	BedrijfIdOrigineel int NULL,
	Actief bit NOT NULL,
	Intern bit NOT NULL,
	ZZP bit NULL,
	Bsn nvarchar(50) NULL,
	Loonkosten decimal(6, 2) NULL,
	[Var] varbinary(MAX) NULL,
	Kvk varbinary(MAX) NULL,
	ProjectleiderId int NULL,
	Adres nvarchar(MAX) NOT NULL,
	Postcode nvarchar(50) NOT NULL,
	Huisnummer nvarchar(50) NOT NULL,
	Plaats nvarchar(MAX) NULL,
	Land nvarchar(MAX) NULL,
	Ophaaladres nvarchar(MAX) NULL,
	Ophaalpostcode nvarchar(50) NULL,
	Ophaalhuisnummer nvarchar(50) NULL,
	Ophaalplaats nvarchar(MAX) NULL,
	Ophaalland nvarchar(MAX) NULL,
	IsChauffeur bit NOT NULL,
	IsChauffeurMa bit NOT NULL,
	IsChauffeurDi bit NOT NULL,
	IsChauffeurWo bit NOT NULL,
	IsChauffeurDo bit NOT NULL,
	IsChauffeurVr bit NOT NULL,
	IsChauffeurZa bit NOT NULL,
	IsChauffeurZo bit NOT NULL,
	IsBijrijder bit NOT NULL,
	IsBijrijderMa bit NOT NULL,
	IsBijrijderDi bit NOT NULL,
	IsBijrijderWo bit NOT NULL,
	IsBijrijderDo bit NOT NULL,
	IsBijrijderVr bit NOT NULL,
	IsBijrijderZa bit NOT NULL,
	IsBijrijderZo bit NOT NULL,
	Kenteken nvarchar(50) NULL,
	Ma decimal(6, 2) NULL,
	Di decimal(6, 2) NULL,
	Wo decimal(6, 2) NULL,
	Do decimal(6, 2) NULL,
	Vr decimal(6, 2) NULL,
	Za decimal(6, 2) NULL,
	Zo decimal(6, 2) NULL,
	Werkweek decimal(6, 2) NULL,
	DefaultBeginuur int NULL,
	DefaultBeginminuut int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Vakman SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Persoon_Loonkosten DEFAULT ((0)) FOR BedrijfId
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Persoon_Actief DEFAULT ((1)) FOR Actief
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurMa DEFAULT ((0)) FOR IsChauffeurMa
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurDi DEFAULT ((0)) FOR IsChauffeurDi
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurWo DEFAULT ((0)) FOR IsChauffeurWo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurDo DEFAULT ((0)) FOR IsChauffeurDo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurVr DEFAULT ((0)) FOR IsChauffeurVr
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurZa DEFAULT ((0)) FOR IsChauffeurZa
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurZo DEFAULT ((0)) FOR IsChauffeurZo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijder DEFAULT ((0)) FOR IsBijrijder
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderMa DEFAULT ((0)) FOR IsBijrijderMa
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderDi DEFAULT ((0)) FOR IsBijrijderDi
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderWo DEFAULT ((0)) FOR IsBijrijderWo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderDo DEFAULT ((0)) FOR IsBijrijderDo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderVr DEFAULT ((0)) FOR IsBijrijderVr
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderZa DEFAULT ((0)) FOR IsBijrijderZa
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsBijrijderZo DEFAULT ((0)) FOR IsBijrijderZo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Persoon_Eigenvervoer DEFAULT ((0)) FOR Kenteken
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_DefaultBeginuur DEFAULT ((8)) FOR DefaultBeginuur
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_DefaultBeginminuut DEFAULT ((0)) FOR DefaultBeginminuut
GO
SET IDENTITY_INSERT dbo.Tmp_Vakman ON
GO
IF EXISTS(SELECT * FROM dbo.Vakman)
	 EXEC('INSERT INTO dbo.Tmp_Vakman (VakmanId, ContactIdOrigineel, BedrijfId, Actief, Intern, ZZP, Bsn, Loonkosten, [Var], Kvk, ProjectleiderId, Adres, Postcode, Huisnummer, Plaats, Land, Ophaaladres, Ophaalpostcode, Ophaalhuisnummer, Ophaalplaats, Ophaalland, IsChauffeur, IsChauffeurMa, IsChauffeurDi, IsChauffeurWo, IsChauffeurDo, IsChauffeurVr, IsChauffeurZa, IsChauffeurZo, IsBijrijder, IsBijrijderMa, IsBijrijderDi, IsBijrijderWo, IsBijrijderDo, IsBijrijderVr, IsBijrijderZa, IsBijrijderZo, Kenteken, Ma, Di, Wo, Do, Vr, Za, Zo, Werkweek, DefaultBeginuur, DefaultBeginminuut)
		SELECT VakmanId, ContactIdOrigineel, BedrijfId, Actief, Intern, ZZP, Bsn, Loonkosten, [Var], Kvk, ProjectleiderId, Adres, Postcode, Huisnummer, Plaats, Land, Ophaaladres, Ophaalpostcode, Ophaalhuisnummer, Ophaalplaats, Ophaalland, IsChauffeur, IsChauffeurMa, IsChauffeurDi, IsChauffeurWo, IsChauffeurDo, IsChauffeurVr, IsChauffeurZa, IsChauffeurZo, IsBijrijder, IsBijrijderMa, IsBijrijderDi, IsBijrijderWo, IsBijrijderDo, IsBijrijderVr, IsBijrijderZa, IsBijrijderZo, Kenteken, Ma, Di, Wo, Do, Vr, Za, Zo, Werkweek, DefaultBeginuur, DefaultBeginminuut FROM dbo.Vakman WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Vakman OFF

GO
IF (OBJECT_ID('FK_Mandagen_Vakman', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Mandagen
	DROP CONSTRAINT FK_Mandagen_Vakman
END

IF (OBJECT_ID('FK_Vakman_Bedrijf', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT FK_Vakman_Bedrijf
END

GO
COMMIT
--DROP TABLE dbo.Vakman
--GO
--EXECUTE sp_rename N'dbo.Tmp_Vakman', N'Vakman', 'OBJECT' 
--GO
--ALTER TABLE dbo.Vakman ADD CONSTRAINT
--	PK_Persoon PRIMARY KEY CLUSTERED 
--	(
--	VakmanId
--	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

--GO
----ALTER TABLE dbo.Vakman ADD CONSTRAINT
----	FK_Vakman_Bedrijf FOREIGN KEY
----	(
----	BedrijfId
----	) REFERENCES dbo.Bedrijf
----	(
----	BedrijfId
----	) ON UPDATE  NO ACTION 
----	 ON DELETE  NO ACTION 
	
--GO
--COMMIT
--select Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
--GO
--ALTER TABLE dbo.Mandagen ADD CONSTRAINT
--	FK_Mandagen_Vakman FOREIGN KEY
--	(
--	VakmanId
--	) REFERENCES dbo.Vakman
--	(
--	VakmanId
--	) ON UPDATE  NO ACTION 
--	 ON DELETE  NO ACTION 
	
--GO
--ALTER TABLE dbo.Mandagen SET (LOCK_ESCALATION = TABLE)
--GO
--COMMIT
--select Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'CONTROL') as Contr_Per 