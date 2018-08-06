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

