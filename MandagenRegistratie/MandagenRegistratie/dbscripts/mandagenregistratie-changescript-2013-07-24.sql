/*
   woensdag 24 juli 201316:55:56
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
ALTER TABLE dbo.Vakman
	DROP CONSTRAINT FK_Vakman_Bedrijf
GO
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
	Ophaaladres nvarchar(MAX) NULL,
	Ophaalpostcode nvarchar(50) NULL,
	Ophaalhuisnummer nvarchar(50) NULL,
	IsChauffeur bit NOT NULL,
	IsChauffeurMa bit NOT NULL,
	IsChauffeurDi bit NOT NULL,
	IsChauffeurWo bit NOT NULL,
	IsChauffeurDo bit NOT NULL,
	IsChauffeurVr bit NOT NULL,
	IsChauffeurZa bit NOT NULL,
	IsChauffeurZo bit NOT NULL,
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
	DF_Vakman_IsChauffeurMa DEFAULT 0 FOR IsChauffeurMa
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurDi DEFAULT 0 FOR IsChauffeurDi
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurWo DEFAULT 0 FOR IsChauffeurWo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurDo DEFAULT 0 FOR IsChauffeurDo
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurVr DEFAULT 0 FOR IsChauffeurVr
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurZa DEFAULT 0 FOR IsChauffeurZa
GO
ALTER TABLE dbo.Tmp_Vakman ADD CONSTRAINT
	DF_Vakman_IsChauffeurZo DEFAULT 0 FOR IsChauffeurZo
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
	 EXEC('INSERT INTO dbo.Tmp_Vakman (VakmanId, ContactIdOrigineel, BedrijfId, Actief, Intern, ZZP, Bsn, Loonkosten, [Var], Kvk, ProjectleiderId, Adres, Postcode, Huisnummer, Ophaaladres, Ophaalpostcode, Ophaalhuisnummer, IsChauffeur, Kenteken, Ma, Di, Wo, Do, Vr, Za, Zo, Werkweek, DefaultBeginuur, DefaultBeginminuut)
		SELECT VakmanId, ContactIdOrigineel, BedrijfId, Actief, Intern, ZZP, Bsn, Loonkosten, [Var], Kvk, ProjectleiderId, Adres, Postcode, Huisnummer, Ophaaladres, Ophaalpostcode, Ophaalhuisnummer, IsChauffeur, Kenteken, Ma, Di, Wo, Do, Vr, Za, Zo, Werkweek, DefaultBeginuur, DefaultBeginminuut FROM dbo.Vakman WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Vakman OFF
GO
ALTER TABLE dbo.Vakmanplanning
	DROP CONSTRAINT FK_Vakmanplanning_Vakman
GO
ALTER TABLE dbo.Route
	DROP CONSTRAINT FK_Route_Vakman
GO
ALTER TABLE dbo.Route
	DROP CONSTRAINT FK_Route_Vakman1
GO
ALTER TABLE dbo.ProjectVakman
	DROP CONSTRAINT FK_ProjectVakman_Vakman
GO
ALTER TABLE dbo.Mandagen
	DROP CONSTRAINT FK_Mandagen_Vakman
GO
DROP TABLE dbo.Vakman
GO
EXECUTE sp_rename N'dbo.Tmp_Vakman', N'Vakman', 'OBJECT' 
GO
ALTER TABLE dbo.Vakman ADD CONSTRAINT
	PK_Persoon PRIMARY KEY CLUSTERED 
	(
	VakmanId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Vakman ADD CONSTRAINT
	FK_Vakman_Bedrijf FOREIGN KEY
	(
	BedrijfId
	) REFERENCES dbo.Bedrijf
	(
	BedrijfId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Vakman', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Mandagen ADD CONSTRAINT
	FK_Mandagen_Vakman FOREIGN KEY
	(
	VakmanId
	) REFERENCES dbo.Vakman
	(
	VakmanId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Mandagen SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Mandagen', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProjectVakman ADD CONSTRAINT
	FK_ProjectVakman_Vakman FOREIGN KEY
	(
	VakmanId
	) REFERENCES dbo.Vakman
	(
	VakmanId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ProjectVakman SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ProjectVakman', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ProjectVakman', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ProjectVakman', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Route ADD CONSTRAINT
	FK_Route_Vakman FOREIGN KEY
	(
	VakmanId
	) REFERENCES dbo.Vakman
	(
	VakmanId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Route ADD CONSTRAINT
	FK_Route_Vakman1 FOREIGN KEY
	(
	ChauffeurId
	) REFERENCES dbo.Vakman
	(
	VakmanId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Route SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Route', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Route', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Route', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Vakmanplanning ADD CONSTRAINT
	FK_Vakmanplanning_Vakman FOREIGN KEY
	(
	VakmanId
	) REFERENCES dbo.Vakman
	(
	VakmanId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Vakmanplanning SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Vakmanplanning', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Vakmanplanning', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Vakmanplanning', 'Object', 'CONTROL') as Contr_Per 