GO
/****** Object:  StoredProcedure [dbo].[p_InsertMDRpersoon]    Script Date: 10/13/2014 17:39:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[p_InsertMDRpersoon]
	@voornaam nvarchar(50)
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ReturnId as int

INSERT INTO MDRpersoon 
VALUES ((SELECT MAX(persoon_ID) FROM MDRpersoon) + 1 , (SELECT MAX(persoon_nr) FROM MDRpersoon) + 1 , null, @voornaam, null, null, null, null, null, null, null, null, null, null, null, null, null)

SET @ReturnId = (SELECT MAX(persoon_ID) FROM MDRpersoon)

RETURN @ReturnId

END





