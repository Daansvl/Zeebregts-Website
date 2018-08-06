USE [PdfOutput]
GO

/****** Object:  View [dbo].[PoortViewVakman]    Script Date: 24-4-2014 9:26:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP VIEW [dbo].[PoortViewVakman]
GO
CREATE VIEW [dbo].[PoortViewVakman]
AS
Select 
	  VakmanId
	, persoon_nr
	, BedrijfId
	, Adres, Postcode, Huisnummer, Plaats
	, voornaam + ' ' + isnull(tussenvoegsel + ' ','') + achternaam AS Naam
	, Bsn
from 
MandagenRegistratie	.dbo.Vakman VKM LEFT JOIN
ZeebregtsDB			.dbo.persoon PRS ON PRS.persoon_ID = VKM.ContactIdOrigineel

GO

