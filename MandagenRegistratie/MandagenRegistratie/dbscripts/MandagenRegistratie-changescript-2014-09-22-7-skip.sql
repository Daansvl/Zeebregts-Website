GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spVakman]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     MVakman.VakmanId, MVakman.ContactIdOrigineel, MVakman.Actief, MVakman.Intern, MVakman.ZZP, MVakman.Bsn, MVakman.Loonkosten, MVakman.[Var], 
                      MVakman.Kvk, MVakman.ProjectleiderId, MVakman.Adres, MVakman.Postcode, MVakman.Huisnummer, MVakman.Ophaaladres, MVakman.Ophaalpostcode, 
                      MVakman.Ophaalhuisnummer, MVakman.IsChauffeur, MVakman.Kenteken, MVakman.Ma, MVakman.Di, MVakman.Wo, MVakman.Do, MVakman.Vr, MVakman.Za, 
                      MVakman.Zo, MVakman.Werkweek, MVakman.DefaultBeginuur, MVakman.DefaultBeginminuut, ZPersoon.voornaam, ZPersoon.tussenvoegsel, ZPersoon.achternaam, 
                      REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, 
                      ZBedrijf.naam AS bedrijfnaam, ZBedrijf.zoeknaam AS bedrijfzoeknaam, ZBedrijf.plaats AS bedrijfplaats, MVakman.Plaats, MVakman.Land, MVakman.Ophaalplaats, 
                      MVakman.Ophaalland, ZPersoon.persoon_nr, ZPersoon.persoon_ID, MVakman.BedrijfIdOrigineel
FROM         dbo.Vakman AS MVakman INNER JOIN
                      Zeebregtsdb.dbo.MDRpersoon AS ZPersoon ON MVakman.ContactIdOrigineel = ZPersoon.persoon_ID INNER JOIN
                      Zeebregtsdb.dbo.MDRbedrijf AS ZBedrijf ON ZPersoon.bedrijf_nr = ZBedrijf.bedrijf_nr
END
ELSE
BEGIN

SELECT     MVakman.VakmanId, MVakman.ContactIdOrigineel, MVakman.Actief, MVakman.Intern, MVakman.ZZP, MVakman.Bsn, MVakman.Loonkosten, MVakman.[Var], 
                      MVakman.Kvk, MVakman.ProjectleiderId, MVakman.Adres, MVakman.Postcode, MVakman.Huisnummer, MVakman.Ophaaladres, MVakman.Ophaalpostcode, 
                      MVakman.Ophaalhuisnummer, MVakman.IsChauffeur, MVakman.Kenteken, MVakman.Ma, MVakman.Di, MVakman.Wo, MVakman.Do, MVakman.Vr, MVakman.Za, 
                      MVakman.Zo, MVakman.Werkweek, MVakman.DefaultBeginuur, MVakman.DefaultBeginminuut, ZPersoon.voornaam, ZPersoon.tussenvoegsel, ZPersoon.achternaam, 
                      REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, 
                      ZBedrijf.naam AS bedrijfnaam, ZBedrijf.zoeknaam AS bedrijfzoeknaam, ZBedrijf.plaats AS bedrijfplaats, MVakman.Plaats, MVakman.Land, MVakman.Ophaalplaats, 
                      MVakman.Ophaalland, ZPersoon.persoon_nr, ZPersoon.persoon_ID, MVakman.BedrijfIdOrigineel
FROM         dbo.Vakman AS MVakman INNER JOIN
                      dbo.MDRpersoon AS ZPersoon ON MVakman.ContactIdOrigineel = ZPersoon.persoon_ID INNER JOIN
                      dbo.MDRbedrijf AS ZBedrijf ON ZPersoon.bedrijf_nr = ZBedrijf.bedrijf_nr
END
GO
/****** Object:  View [dbo].[vwProjectAll]    Script Date: 09/22/2014 13:14:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spProjectAll]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     ZPersoon.persoon_ID, ZPersoon.persoon_nr, ZPersoon.bedrijf_nr, ZPersoon.man, ZPersoon.voorletters, ZPersoon.voornaam, ZPersoon.tussenvoegsel, 
                      ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') 
                      AS ZPersoonFullname, ZProject.project_NR AS ProjectNrOrigineel, ZBedrijf.naam AS Bedrijfsnaam, ZProject.naam_project, ZProject.plaats
FROM         dbo.Project AS MProject CROSS JOIN
                      Zeebregtsdb.dbo.MDRpersoon AS ZPersoon INNER JOIN
                      dbo.Gebruiker AS MGebruiker ON MProject.ProjectleiderId = MGebruiker.ProjectleiderId AND MGebruiker.ContactIdOrigineel = ZPersoon.persoon_ID RIGHT OUTER JOIN
                      Zeebregtsdb.dbo.MDRproject AS ZProject ON ZProject.project_NR = MProject.ProjectNr INNER JOIN
                      Zeebregtsdb.dbo.MDRbedrijf AS ZBedrijf ON ZProject.opdrachtgeverZEEBREGTS_nr = ZBedrijf.bedrijf_nr
END
ELSE
BEGIN

SELECT     ZPersoon.persoon_ID, ZPersoon.persoon_nr, ZPersoon.bedrijf_nr, ZPersoon.man, ZPersoon.voorletters, ZPersoon.voornaam, ZPersoon.tussenvoegsel, 
                      ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') 
                      AS ZPersoonFullname, ZProject.project_NR AS ProjectNrOrigineel, ZBedrijf.naam AS Bedrijfsnaam, ZProject.naam_project, ZProject.plaats
FROM         dbo.Project AS MProject CROSS JOIN
                      dbo.MDRpersoon AS ZPersoon INNER JOIN
                      dbo.Gebruiker AS MGebruiker ON MProject.ProjectleiderId = MGebruiker.ProjectleiderId AND MGebruiker.ContactIdOrigineel = ZPersoon.persoon_ID RIGHT OUTER JOIN
                      dbo.MDRproject AS ZProject ON ZProject.project_NR = MProject.ProjectNr INNER JOIN
                      dbo.MDRbedrijf AS ZBedrijf ON ZProject.opdrachtgeverZEEBREGTS_nr = ZBedrijf.bedrijf_nr
END
GO
/****** Object:  View [dbo].[vwProject]    Script Date: 09/22/2014 13:14:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spProject]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     MProject.ProjectId, MProject.Naam, MProject.ProjectleiderId, MProject.Actief, MProject.Mutatiedatum, MProject.Postcode, MProject.Huisnummer, MProject.Adres, 
                      ZPersoon.persoon_ID, ZPersoon.persoon_nr, ZPersoon.bedrijf_nr, ZPersoon.man, ZPersoon.voorletters, ZPersoon.voornaam, ZPersoon.tussenvoegsel, 
                      ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') 
                      AS ZPersoonFullname, ZBedrijf.naam AS Bedrijfsnaam, ZProject.project_NR AS ProjectNrOrigineel, ZProject.naam_project AS ZProjectNaam, 
                      ZProject.plaats AS Projectplaats, ZBedrijf.plaats AS Bedrijfsplaats
FROM         dbo.Project AS MProject CROSS JOIN
                      Zeebregtsdb.dbo.MDRpersoon AS ZPersoon INNER JOIN
                      dbo.Gebruiker AS MGebruiker ON MProject.ProjectleiderId = MGebruiker.ProjectleiderId AND MGebruiker.ContactIdOrigineel = ZPersoon.persoon_ID INNER JOIN
                      Zeebregtsdb.dbo.MDRproject AS ZProject ON ZProject.project_NR = MProject.ProjectNr INNER JOIN
                      Zeebregtsdb.dbo.MDRbedrijf AS ZBedrijf ON ZProject.opdrachtgeverZEEBREGTS_nr = ZBedrijf.bedrijf_nr
END
ELSE
BEGIN

SELECT     MProject.ProjectId, MProject.Naam, MProject.ProjectleiderId, MProject.Actief, MProject.Mutatiedatum, MProject.Postcode, MProject.Huisnummer, MProject.Adres, 
                      ZPersoon.persoon_ID, ZPersoon.persoon_nr, ZPersoon.bedrijf_nr, ZPersoon.man, ZPersoon.voorletters, ZPersoon.voornaam, ZPersoon.tussenvoegsel, 
                      ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') 
                      AS ZPersoonFullname, ZBedrijf.naam AS Bedrijfsnaam, ZProject.project_NR AS ProjectNrOrigineel, ZProject.naam_project AS ZProjectNaam, 
                      ZProject.plaats AS Projectplaats, ZBedrijf.plaats AS Bedrijfsplaats
FROM         dbo.Project AS MProject CROSS JOIN
                      dbo.MDRpersoon AS ZPersoon INNER JOIN
                      dbo.Gebruiker AS MGebruiker ON MProject.ProjectleiderId = MGebruiker.ProjectleiderId AND MGebruiker.ContactIdOrigineel = ZPersoon.persoon_ID INNER JOIN
                      dbo.MDRproject AS ZProject ON ZProject.project_NR = MProject.ProjectNr INNER JOIN
                      dbo.MDRbedrijf AS ZBedrijf ON ZProject.opdrachtgeverZEEBREGTS_nr = ZBedrijf.bedrijf_nr
END
GO
/****** Object:  View [dbo].[vwContactAll]    Script Date: 09/22/2014 13:14:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spContactAll]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     ZPersoon.persoon_nr, ZPersoon.persoon_ID, ZPersoon.voornaam, ZPersoon.tussenvoegsel, ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') 
                      + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, ZBedrijf.naam AS bedrijfnaam, 
                      ZBedrijf.zoeknaam AS bedrijfzoeknaam, ZBedrijf.plaats AS bedrijfplaats, ZBedrijf.bedrijf_nr
FROM         Zeebregtsdb.dbo.MDRpersoon AS ZPersoon INNER JOIN
                      Zeebregtsdb.dbo.MDRbedrijf AS ZBedrijf ON ZPersoon.bedrijf_nr = ZBedrijf.bedrijf_nr
END
ELSE
BEGIN

SELECT     ZPersoon.persoon_nr, ZPersoon.persoon_ID, ZPersoon.voornaam, ZPersoon.tussenvoegsel, ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') 
                      + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, ZBedrijf.naam AS bedrijfnaam, 
                      ZBedrijf.zoeknaam AS bedrijfzoeknaam, ZBedrijf.plaats AS bedrijfplaats, ZBedrijf.bedrijf_nr
FROM         dbo.MDRpersoon AS ZPersoon INNER JOIN
                      dbo.MDRbedrijf AS ZBedrijf ON ZPersoon.bedrijf_nr = ZBedrijf.bedrijf_nr
END
GO
/****** Object:  View [dbo].[vwVakmanRelaties]    Script Date: 09/22/2014 13:14:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spVakmanRelaties]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     TOP (100) PERCENT JV.VakmanId, ZP.persoon_nr AS PersoonNR, ZP.voornaam + ISNULL(' ' + ZP.tussenvoegsel + ' ', ' ') + ZP.achternaam AS PersoonNaam, 
                      ZB.bedrijf_nr AS BedrijfNR, ZB.naam AS BedrijfNaam, JV.Intern, JV.ZZP, 
                      CAST(CASE WHEN JV.Intern = 1 THEN 'Uitzendkracht' WHEN JV.ZZP = 1 THEN 'ZZP' ELSE 'Intern' END AS varchar) AS ArbeidsRelatie, 
                      CAST(CASE WHEN (ZB.bedrijf_nr IN (59, 153, 394, 469, 520, 542, 587, 650)) 
                      THEN '(Extern) Per meter' ELSE CAST(CASE WHEN JV.Intern = 1 THEN 'Per uur' WHEN JV.ZZP = 1 THEN 'Per meter' ELSE 'Per uur' END AS varchar) END AS varchar) 
                      AS Contract, CAST(CASE WHEN JV.VakmanId IN (147, 148) THEN 'Projectleiding' WHEN JV.VakmanId IN (162, 159, 158, 161, 160) 
                      THEN 'Werkvoorbereiding' WHEN JV.VakmanId IN (149, 157) THEN 'Management' WHEN ZP.persoon_nr IN (1456, 801, 1354) 
                      THEN 'Programmeur' WHEN ZP.persoon_nr IN (1590) THEN 'Administratie' ELSE 'Vakman' END AS varchar) AS Functie, CAST(CASE WHEN ZB.bedrijf_nr IN (59, 153, 
                      394, 469, 520, 542, 587, 650) THEN ZB.naam ELSE 'Zeebregts' END AS varchar) AS KetenPartner, MIN(Mand.Begintijd) AS Start, MAX(Mand.Eindtijd) AS Eind
FROM         dbo.Vakman AS JV INNER JOIN
                      Zeebregtsdb.dbo.MDRpersoon AS ZP ON JV.ContactIdOrigineel = ZP.persoon_ID INNER JOIN
                      Zeebregtsdb.dbo.MDRbedrijf AS ZB ON ZP.bedrijf_nr = ZB.bedrijf_nr LEFT OUTER JOIN
                      dbo.Mandagen AS Mand ON Mand.VakmanId = JV.VakmanId

GROUP BY JV.VakmanId, ZP.persoon_nr, ZP.voornaam, ZP.tussenvoegsel, ZP.achternaam, ZB.bedrijf_nr, ZB.naam, JV.Intern, JV.ZZP
ORDER BY JV.VakmanId
END
ELSE
BEGIN

SELECT     TOP (100) PERCENT JV.VakmanId, ZP.persoon_nr AS PersoonNR, ZP.voornaam + ISNULL(' ' + ZP.tussenvoegsel + ' ', ' ') + ZP.achternaam AS PersoonNaam, 
                      ZB.bedrijf_nr AS BedrijfNR, ZB.naam AS BedrijfNaam, JV.Intern, JV.ZZP, 
                      CAST(CASE WHEN JV.Intern = 1 THEN 'Uitzendkracht' WHEN JV.ZZP = 1 THEN 'ZZP' ELSE 'Intern' END AS varchar) AS ArbeidsRelatie, 
                      CAST(CASE WHEN (ZB.bedrijf_nr IN (59, 153, 394, 469, 520, 542, 587, 650)) 
                      THEN '(Extern) Per meter' ELSE CAST(CASE WHEN JV.Intern = 1 THEN 'Per uur' WHEN JV.ZZP = 1 THEN 'Per meter' ELSE 'Per uur' END AS varchar) END AS varchar) 
                      AS Contract, CAST(CASE WHEN JV.VakmanId IN (147, 148) THEN 'Projectleiding' WHEN JV.VakmanId IN (162, 159, 158, 161, 160) 
                      THEN 'Werkvoorbereiding' WHEN JV.VakmanId IN (149, 157) THEN 'Management' WHEN ZP.persoon_nr IN (1456, 801, 1354) 
                      THEN 'Programmeur' WHEN ZP.persoon_nr IN (1590) THEN 'Administratie' ELSE 'Vakman' END AS varchar) AS Functie, CAST(CASE WHEN ZB.bedrijf_nr IN (59, 153, 
                      394, 469, 520, 542, 587, 650) THEN ZB.naam ELSE 'Zeebregts' END AS varchar) AS KetenPartner, MIN(Mand.Begintijd) AS Start, MAX(Mand.Eindtijd) AS Eind
FROM         dbo.Vakman AS JV INNER JOIN
                      dbo.MDRpersoon AS ZP ON JV.ContactIdOrigineel = ZP.persoon_ID INNER JOIN
                      dbo.MDRbedrijf AS ZB ON ZP.bedrijf_nr = ZB.bedrijf_nr LEFT OUTER JOIN
                      dbo.Mandagen AS Mand ON Mand.VakmanId = JV.VakmanId

GROUP BY JV.VakmanId, ZP.persoon_nr, ZP.voornaam, ZP.tussenvoegsel, ZP.achternaam, ZB.bedrijf_nr, ZB.naam, JV.Intern, JV.ZZP
ORDER BY JV.VakmanId
END
GO
/****** Object:  View [dbo].[vwMandagenBeta]    Script Date: 09/22/2014 13:14:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMandagenBeta]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     M.ProjectId, M.VakmanId, V.ContactIdOrigineel, M.Begintijd, M.Eindtijd, M.Status, M.Uren, M.Minuten, M.ProjectleiderId, M.Geannulleerd, M.Bevestigd, 
                      M.Mutatiedatum, M.MutatieDoorProjectleiderId, P.Naam, cp.achternaam, cp.voornaam, cp.tussenvoegsel, G.Gebruikersnaam, pr.project_NR, pr.naam_project
FROM         dbo.Mandagen AS M INNER JOIN
                      dbo.Project AS P ON M.ProjectId = P.ProjectId INNER JOIN
                      dbo.Vakman AS V ON M.VakmanId = V.VakmanId INNER JOIN
                      Zeebregtsdb.dbo.MDRpersoon AS cp ON cp.persoon_ID = V.ContactIdOrigineel INNER JOIN
                      dbo.Gebruiker AS G ON G.ProjectleiderId = M.ProjectleiderId INNER JOIN
                      Zeebregtsdb.dbo.MDRproject AS pr ON pr.project_NR = P.ProjectNr
END
ELSE
BEGIN

SELECT     M.ProjectId, M.VakmanId, V.ContactIdOrigineel, M.Begintijd, M.Eindtijd, M.Status, M.Uren, M.Minuten, M.ProjectleiderId, M.Geannulleerd, M.Bevestigd, 
                      M.Mutatiedatum, M.MutatieDoorProjectleiderId, P.Naam, cp.achternaam, cp.voornaam, cp.tussenvoegsel, G.Gebruikersnaam, pr.project_NR, pr.naam_project
FROM         dbo.Mandagen AS M INNER JOIN
                      dbo.Project AS P ON M.ProjectId = P.ProjectId INNER JOIN
                      dbo.Vakman AS V ON M.VakmanId = V.VakmanId INNER JOIN
                      dbo.MDRpersoon AS cp ON cp.persoon_ID = V.ContactIdOrigineel INNER JOIN
                      dbo.Gebruiker AS G ON G.ProjectleiderId = M.ProjectleiderId INNER JOIN
                      dbo.MDRproject AS pr ON pr.project_NR = P.ProjectNr
END
GO
/****** Object:  View [dbo].[vwMandagen]    Script Date: 09/22/2014 13:14:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMandagen]
AS
IF EXISTS(SELECT     ConfigurationValue
                         FROM         dbo.Configuration
                         WHERE     ConfigurationName = 'ForeignDatabase' AND ConfigurationValue = 'Zeebregts')
BEGIN

SELECT     M.ProjectId, M.VakmanId, V.ContactIdOrigineel, M.Begintijd, M.Eindtijd, M.Status, M.Uren, M.Minuten, M.ProjectleiderId, M.Geannulleerd, M.Bevestigd, 
                      M.Mutatiedatum, M.MutatieDoorProjectleiderId, P.Naam, cp.achternaam, cp.voornaam, cp.tussenvoegsel, G.Gebruikersnaam, pr.project_NR, pr.naam_project
FROM         dbo.Mandagen AS M INNER JOIN
                      dbo.Project AS P ON M.ProjectId = P.ProjectId INNER JOIN
                      dbo.Vakman AS V ON M.VakmanId = V.VakmanId INNER JOIN
                      Zeebregtsdb.dbo.MDRpersoon AS cp ON cp.persoon_ID = V.ContactIdOrigineel INNER JOIN
                      dbo.Gebruiker AS G ON G.ProjectleiderId = M.ProjectleiderId INNER JOIN
                      Zeebregtsdb.dbo.MDRproject AS pr ON pr.project_NR = P.ProjectNr
END
ELSE
BEGIN

SELECT     M.ProjectId, M.VakmanId, V.ContactIdOrigineel, M.Begintijd, M.Eindtijd, M.Status, M.Uren, M.Minuten, M.ProjectleiderId, M.Geannulleerd, M.Bevestigd, 
                      M.Mutatiedatum, M.MutatieDoorProjectleiderId, P.Naam, cp.achternaam, cp.voornaam, cp.tussenvoegsel, G.Gebruikersnaam, pr.project_NR, pr.naam_project
FROM         dbo.Mandagen AS M INNER JOIN
                      dbo.Project AS P ON M.ProjectId = P.ProjectId INNER JOIN
                      dbo.Vakman AS V ON M.VakmanId = V.VakmanId INNER JOIN
                      dbo.MDRpersoon AS cp ON cp.persoon_ID = V.ContactIdOrigineel INNER JOIN
                      dbo.Gebruiker AS G ON G.ProjectleiderId = M.ProjectleiderId INNER JOIN
                      dbo.MDRproject AS pr ON pr.project_NR = P.ProjectNr
END
GO
