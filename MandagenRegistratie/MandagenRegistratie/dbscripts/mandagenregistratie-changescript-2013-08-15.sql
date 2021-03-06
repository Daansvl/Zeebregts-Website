USE [MandagenRegistratieBeta]
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPaneCount' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwVakman'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwVakman'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPaneCount' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProjectAll'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProjectAll'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProjectAll'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPaneCount' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProject'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProject'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProject'

GO
/****** Object:  View [dbo].[vwVakman]    Script Date: 15-8-2013 18:08:59 ******/
DROP VIEW [dbo].[vwVakman]
GO
/****** Object:  View [dbo].[vwProjectAll]    Script Date: 15-8-2013 18:08:59 ******/
DROP VIEW [dbo].[vwProjectAll]
GO
/****** Object:  View [dbo].[vwProject]    Script Date: 15-8-2013 18:08:59 ******/
DROP VIEW [dbo].[vwProject]
GO
/****** Object:  View [dbo].[vwProject]    Script Date: 15-8-2013 18:08:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwProject]
AS
SELECT        MProject.ProjectId, MProject.ProjectIdOrigineel, MProject.Naam, MProject.ProjectleiderId, MProject.Actief, MProject.Mutatiedatum, MProject.Postcode, 
                         MProject.Huisnummer, MProject.Adres, ZPersoon.persoon_ID, ZPersoon.persoon_nr, ZPersoon.taak_id, ZPersoon.bedrijf_nr, ZPersoon.man, 
                         ZPersoon.voorletters, ZPersoon.voornaam, ZPersoon.tussenvoegsel, ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') 
                         + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, ZPersoon.zamobiel, 
                         ZPersoon.zatelefoonvast, ZPersoon.zafax, ZPersoon.zaemail, ZPersoon.geboortedatum, ZPersoon.vastevrijedag1, ZPersoon.vastevrijedag2, 
                         ZPersoon.vastevrijedag3, ZPersoon.NIETactief, ZPersoon.adres_id, ZPersoon.telefoon_nr_settings, ZPersoon.telefoon_nr_1, ZPersoon.telefoon_nr_2, 
                         ZPersoon.telefoon_nr_3, ZBedrijf.naam AS Bedrijfsnaam, ZProject.project_NR AS ProjectNrOrigineel, ZProject.naam_project AS ZProjectNaam
FROM            dbo.Project AS MProject CROSS JOIN
                         zeebregtsdb.dbo.persoon AS ZPersoon INNER JOIN
                         dbo.Projectleider AS MProjectleider ON MProject.ProjectleiderId = MProjectleider.ProjectleiderId AND 
                         MProjectleider.ContactIdOrigineel = ZPersoon.persoon_ID INNER JOIN
                         zeebregtsdb.dbo.project AS ZProject ON ZProject.project_ID = MProject.ProjectIdOrigineel INNER JOIN
                         zeebregtsdb.dbo.bedrijf AS ZBedrijf ON ZProject.opdrachtgeverZEEBREGTS_nr = ZBedrijf.bedrijf_nr

GO
/****** Object:  View [dbo].[vwProjectAll]    Script Date: 15-8-2013 18:08:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwProjectAll]
AS
SELECT        ZPersoon.persoon_ID, ZPersoon.persoon_nr, ZPersoon.taak_id, ZPersoon.bedrijf_nr, ZPersoon.man, ZPersoon.voorletters, ZPersoon.voornaam, 
                         ZPersoon.tussenvoegsel, ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') 
                         + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, ZPersoon.zamobiel, ZPersoon.zatelefoonvast, ZPersoon.zafax, ZPersoon.zaemail, 
                         ZPersoon.geboortedatum, ZPersoon.vastevrijedag1, ZPersoon.vastevrijedag2, ZPersoon.vastevrijedag3, ZPersoon.NIETactief, ZPersoon.adres_id, 
                         ZPersoon.telefoon_nr_settings, ZPersoon.telefoon_nr_1, ZPersoon.telefoon_nr_2, ZPersoon.telefoon_nr_3, ZProject.project_NR AS ProjectNrOrigineel, 
                         ZBedrijf.naam AS Bedrijfsnaam, ZProject.naam_project, ZProject.project_ID
FROM            dbo.Project AS MProject CROSS JOIN
                         zeebregtsdb.dbo.persoon AS ZPersoon INNER JOIN
                         dbo.Projectleider AS MProjectleider ON MProject.ProjectleiderId = MProjectleider.ProjectleiderId AND 
                         MProjectleider.ContactIdOrigineel = ZPersoon.persoon_ID RIGHT OUTER JOIN
                         zeebregtsdb.dbo.project AS ZProject ON ZProject.project_ID = MProject.ProjectIdOrigineel INNER JOIN
                         zeebregtsdb.dbo.bedrijf AS ZBedrijf ON ZProject.opdrachtgeverZEEBREGTS_nr = ZBedrijf.bedrijf_nr

GO
/****** Object:  View [dbo].[vwVakman]    Script Date: 15-8-2013 18:08:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwVakman]
AS
SELECT        MVakman.VakmanId, MVakman.ContactIdOrigineel, MVakman.BedrijfId, MVakman.Actief, MVakman.Intern, MVakman.ZZP, MVakman.Bsn, 
                         MVakman.Loonkosten, MVakman.[Var], MVakman.Kvk, MVakman.ProjectleiderId, MVakman.Adres, MVakman.Postcode, MVakman.Huisnummer, 
                         MVakman.Ophaaladres, MVakman.Ophaalpostcode, MVakman.Ophaalhuisnummer, MVakman.IsChauffeur, MVakman.Kenteken, MVakman.Ma, MVakman.Di, 
                         MVakman.Wo, MVakman.Do, MVakman.Vr, MVakman.Za, MVakman.Zo, MVakman.Werkweek, MVakman.DefaultBeginuur, MVakman.DefaultBeginminuut, 
                         ZPersoon.voornaam, ZPersoon.tussenvoegsel, ZPersoon.achternaam, REPLACE(ISNULL(ZPersoon.voornaam, '') + ' ' + ISNULL(ZPersoon.tussenvoegsel, '') 
                         + ' ' + ISNULL(ZPersoon.achternaam, ''), '  ', ' ') AS ZPersoonFullname, ZBedrijf.naam AS bedrijfnaam, ZBedrijf.zoeknaam AS bedrijfzoeknaam, 
                         ZBedrijf.plaats AS bedrijfplaats
FROM            dbo.Vakman AS MVakman INNER JOIN
                         zeebregtsdb.dbo.persoon AS ZPersoon ON MVakman.ContactIdOrigineel = ZPersoon.persoon_ID INNER JOIN
                         zeebregtsdb.dbo.bedrijf AS ZBedrijf ON ZPersoon.bedrijf_nr = ZBedrijf.bedrijf_nr

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[15] 2[26] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "MProject"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 222
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZPersoon"
            Begin Extent = 
               Top = 6
               Left = 578
               Bottom = 136
               Right = 773
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MProjectleider"
            Begin Extent = 
               Top = 17
               Left = 351
               Bottom = 130
               Right = 540
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZProject"
            Begin Extent = 
               Top = 6
               Left = 811
               Bottom = 136
               Right = 1107
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZBedrijf"
            Begin Extent = 
               Top = 6
               Left = 1145
               Bottom = 136
               Right = 1347
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 37
         Width = 284
         Width = 5445
         Width = 5460
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
    ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     Width = 1500
         Width = 1500
         Width = 2580
         Width = 1500
         Width = 1500
         Width = 2385
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[42] 4[20] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "MProject"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 251
               Right = 222
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZPersoon"
            Begin Extent = 
               Top = 6
               Left = 260
               Bottom = 136
               Right = 455
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MProjectleider"
            Begin Extent = 
               Top = 6
               Left = 493
               Bottom = 119
               Right = 682
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZProject"
            Begin Extent = 
               Top = 6
               Left = 720
               Bottom = 136
               Right = 1016
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZBedrijf"
            Begin Extent = 
               Top = 6
               Left = 1054
               Bottom = 136
               Right = 1256
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 36
         Width = 284
         Width = 1500
         Width = 2640
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
     ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProjectAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4140
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProjectAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwProjectAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "MVakman"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 234
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZPersoon"
            Begin Extent = 
               Top = 6
               Left = 272
               Bottom = 136
               Right = 467
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZBedrijf"
            Begin Extent = 
               Top = 6
               Left = 505
               Bottom = 136
               Right = 707
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3240
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwVakman'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwVakman'
GO
