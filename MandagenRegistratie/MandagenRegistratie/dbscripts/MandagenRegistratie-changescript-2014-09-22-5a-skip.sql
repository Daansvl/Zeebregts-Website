
 ALTER TABLE Configuration
  ALTER COLUMN ConfigurationName
    NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

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


