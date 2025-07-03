-- Disable constraint checking
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"

-- Drop all foreign key constraints
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'
ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))
    + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
    ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.foreign_keys;
EXEC sp_executesql @sql;

-- Drop tables in reverse order of dependencies
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SpectaclesGroupes') DROP TABLE [SpectaclesGroupes];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ArtistesSpectacles') DROP TABLE [ArtistesSpectacles];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TarifsSpectacles') DROP TABLE [TarifsSpectacles];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TarifsGroupes') DROP TABLE [TarifsGroupes];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Billets') DROP TABLE [Billets];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GroupesSpectaclesOrganisation') DROP TABLE [GroupesSpectaclesOrganisation];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Programmation') DROP TABLE [Programmation];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Spectacles') DROP TABLE [Spectacles];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Artistes') DROP TABLE [Artistes];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GroupesSpectacles') DROP TABLE [GroupesSpectacles];
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TypesTarifs') DROP TABLE [TypesTarifs];

-- Drop the migrations history table
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory') DROP TABLE [__EFMigrationsHistory]; 