GO
USE [master]
GO
ALTER DATABASE MandagenRegistratieBeta SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

GO
ALTER DATABASE MandagenRegistratieBeta COLLATE SQL_Latin1_General_CP1_CI_AS -- SQL_Latin1_General_CP1_CI_AS
GO
ALTER DATABASE MandagenRegistratieBeta SET MULTI_USER WITH ROLLBACK IMMEDIATE;
Go

	Use MandagenRegistratieBeta

-- DOE DIT HANDMATIG VOOR ALLE KOLOMMEN

--SELECT
--    col.name, col.collation_name
--FROM 
--    sys.columns col
--WHERE
--    object_id = OBJECT_ID('Configuration')

-- ALTER TABLE Configuration
--  ALTER COLUMN ConfigurationValue
--    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

GO

 ALTER TABLE Configuration
  ALTER COLUMN ConfigurationName
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

	GO
 ALTER TABLE Configuration
  ALTER COLUMN ConfigurationValue
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Gebruiker
  ALTER COLUMN Gebruikersnaam
    NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Gebruiker
  ALTER COLUMN Windowsidentity
    NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Log
  ALTER COLUMN Ipaddress
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Log
  ALTER COLUMN Message
    NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Loggen
  ALTER COLUMN Ipaddress
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Loggen
  ALTER COLUMN Message
    NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Loonkosten
  ALTER COLUMN Achternaam
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Loonkosten
  ALTER COLUMN Voornaam
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Mandagen
  ALTER COLUMN KentekenHeen
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Mandagen
  ALTER COLUMN KentekenTerug
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE MandagenArchief
  ALTER COLUMN KentekenHeen
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE MandagenArchief
  ALTER COLUMN KentekenTerug
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Mutatie
  ALTER COLUMN Originelewaarde
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Mutatie
  ALTER COLUMN Nieuwewaarde
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Mutatietype
  ALTER COLUMN Naam
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Project
  ALTER COLUMN Naam
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Project
  ALTER COLUMN Postcode
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Project
  ALTER COLUMN Huisnummer
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Project
  ALTER COLUMN Adres
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Setting
  ALTER COLUMN SettingsNaam
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Setting
  ALTER COLUMN SettingsValue
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Bsn
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Adres
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Vakman
  ALTER COLUMN Postcode
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Vakman
  ALTER COLUMN Huisnummer
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Vakman
  ALTER COLUMN Plaats
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Land
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Ophaaladres
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Ophaalpostcode
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Ophaalhuisnummer
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Ophaalplaats
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Ophaalland
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakman
  ALTER COLUMN Kenteken
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

 ALTER TABLE Vakmansoort
  ALTER COLUMN Omschrijving
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

 ALTER TABLE Vakmanstatus
  ALTER COLUMN Omschrijving
    NVARCHAR(Max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL


Go


/****** Object:  Table [dbo].[MDRadressen]    Script Date: 22-9-2014 12:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MDRadressen](
	[adres_id] [int] NOT NULL,
	[land] [nvarchar](50) NULL,
	[plaats] [nvarchar](50) NULL,
	[huisnummer_toevoeging] [nvarchar](50) NULL,
	[straat_1] [nvarchar](100) NULL,
	[straat_2] [nvarchar](100) NULL,
	[postcode_cijfers] [int] NULL,
	[postcode_letters] [nvarchar](2) NULL,
	[huis_postbus_nummer] [nvarchar](50) NULL,
 CONSTRAINT [PK_MDRadressen] PRIMARY KEY CLUSTERED 
(
	[adres_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MDRbedrijf]    Script Date: 22-9-2014 12:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MDRbedrijf](
	[bedrijf_ID] [int] NOT NULL,
	[bedrijf_nr] [int] NULL,
	[naam] [nvarchar](150) NULL,
	[zoeknaam] [nvarchar](52) NULL,
	[plaats] [nvarchar](50) NULL,
	[adres_id_bezoek] [int] NULL,
	[adres_id_post] [int] NULL,
 CONSTRAINT [PK_MDRbedrijf] PRIMARY KEY CLUSTERED 
(
	[bedrijf_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MDRpersoon]    Script Date: 22-9-2014 12:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MDRpersoon](
	[persoon_ID] [int] NOT NULL,
	[persoon_nr] [int] NULL,
	[voorletters] [nvarchar](50) NULL,
	[voornaam] [nvarchar](50) NULL,
	[tussenvoegsel] [nvarchar](50) NULL,
	[achternaam] [nvarchar](50) NULL,
	[man] [bit] NULL,
	[bedrijf_nr] [int] NULL,
	[telefoon_nr_1] [nvarchar](50) NULL,
	[telefoon_nr_2] [nvarchar](50) NULL,
	[telefoon_nr_3] [nvarchar](50) NULL,
	[adres_id] [int] NULL,
	[geboortedatum] [datetime] NULL,
	[zaemail] [nvarchar](50) NULL,
	[zamobiel] [nvarchar](50) NULL,
	[zatelefoonvast] [nvarchar](50) NULL,
	[zafax] [nvarchar](50) NULL,
 CONSTRAINT [PK_MDRpersoon] PRIMARY KEY CLUSTERED 
(
	[persoon_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MDRproject]    Script Date: 22-9-2014 12:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MDRproject](
	[project_ID] [int] NOT NULL,
	[project_NR] [int] NULL,
	[naam_project] [nvarchar](100) NULL,
	[plaats] [nvarchar](50) NULL,
	[opdrachtgeverZEEBREGTS_nr] [int] NULL,
	[adres_id_bouw] [int] NULL,
	[aannemer_projectnummer] [nvarchar](50) NULL,
	[aannemer_contractnummer] [nvarchar](50) NULL,
	[nacalculatiecode] [nvarchar](50) NULL,
	[adres_id_factuur] [int] NULL,
 CONSTRAINT [PK_MDRproject] PRIMARY KEY CLUSTERED 
(
	[project_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
Insert into [dbo].[MDRadressen]
SELECT * from [ZeebregtsDb].[dbo].[MDRAdressen]

Insert into [dbo].[MDRbedrijf]
SELECT * from [ZeebregtsDb].[dbo].[MDRbedrijf]

Insert into [dbo].[MDRproject]
SELECT * from [ZeebregtsDb].[dbo].[MDRproject]

Insert into [dbo].[MDRpersoon]
SELECT * from [ZeebregtsDb].[dbo].[MDRpersoon]




GO
