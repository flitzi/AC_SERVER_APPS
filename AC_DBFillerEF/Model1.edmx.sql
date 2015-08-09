
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/09/2015 10:10:59
-- Generated from EDMX file: C:\Users\Tom\Documents\GitHub\AC_SERVER_APPS\AC_DBFillerEF\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AC_DB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Laps_To_Drivers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Laps] DROP CONSTRAINT [FK_Laps_To_Drivers];
GO
IF OBJECT_ID(N'[dbo].[FK_Laps_To_Sessions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Laps] DROP CONSTRAINT [FK_Laps_To_Sessions];
GO
IF OBJECT_ID(N'[dbo].[FK_SessionResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Results] DROP CONSTRAINT [FK_SessionResult];
GO
IF OBJECT_ID(N'[dbo].[FK_DriverResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Results] DROP CONSTRAINT [FK_DriverResult];
GO
IF OBJECT_ID(N'[dbo].[FK_SessionIncidents]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Incidents] DROP CONSTRAINT [FK_SessionIncidents];
GO
IF OBJECT_ID(N'[dbo].[FK_DriverIncident1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Incidents] DROP CONSTRAINT [FK_DriverIncident1];
GO
IF OBJECT_ID(N'[dbo].[FK_DriverIncidents2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Incidents] DROP CONSTRAINT [FK_DriverIncidents2];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Drivers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Drivers];
GO
IF OBJECT_ID(N'[dbo].[Laps]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Laps];
GO
IF OBJECT_ID(N'[dbo].[Sessions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sessions];
GO
IF OBJECT_ID(N'[dbo].[Results]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Results];
GO
IF OBJECT_ID(N'[dbo].[Incidents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Incidents];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Drivers'
CREATE TABLE [dbo].[Drivers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SteamId] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Team] nvarchar(max)  NULL,
    [IncidentCount] int  NOT NULL,
    [Distance] int  NOT NULL,
    [Points] int  NOT NULL
);
GO

-- Creating table 'Laps'
CREATE TABLE [dbo].[Laps] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SessionId] int  NOT NULL,
    [DriverId] int  NOT NULL,
    [Car] nvarchar(max)  NOT NULL,
    [LapNo] smallint  NOT NULL,
    [Time] int  NOT NULL,
    [Cuts] smallint  NOT NULL,
    [Position] smallint  NOT NULL,
    [Grip] real  NOT NULL,
    [Timestamp] datetime  NOT NULL
);
GO

-- Creating table 'Sessions'
CREATE TABLE [dbo].[Sessions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Server] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Type] tinyint  NOT NULL,
    [Track] nvarchar(max)  NOT NULL,
    [LapCount] smallint  NULL,
    [Time] int  NULL,
    [Ambient] tinyint  NOT NULL,
    [Road] tinyint  NOT NULL,
    [Weather] nvarchar(max)  NOT NULL,
    [Timestamp] datetime  NOT NULL
);
GO

-- Creating table 'Results'
CREATE TABLE [dbo].[Results] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SessionId] int  NOT NULL,
    [DriverId] int  NOT NULL,
    [Car] nvarchar(max)  NOT NULL,
    [StartPosition] smallint  NOT NULL,
    [Position] smallint  NOT NULL,
    [IncidentCount] int  NOT NULL,
    [Distance] int  NOT NULL,
    [LapCount] smallint  NOT NULL,
    [Gap] nvarchar(max)  NULL,
    [TopSpeed] smallint  NOT NULL
);
GO

-- Creating table 'Incidents'
CREATE TABLE [dbo].[Incidents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SessionId] int  NOT NULL,
    [Type] tinyint  NOT NULL,
    [RelativeSpeed] real  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [DriverId1] int  NOT NULL,
    [DriverId2] int  NULL,
    [WorldPosX] real  NOT NULL,
    [WorldPosY] real  NOT NULL,
    [WorldPosZ] real  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Drivers'
ALTER TABLE [dbo].[Drivers]
ADD CONSTRAINT [PK_Drivers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Laps'
ALTER TABLE [dbo].[Laps]
ADD CONSTRAINT [PK_Laps]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sessions'
ALTER TABLE [dbo].[Sessions]
ADD CONSTRAINT [PK_Sessions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Results'
ALTER TABLE [dbo].[Results]
ADD CONSTRAINT [PK_Results]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Incidents'
ALTER TABLE [dbo].[Incidents]
ADD CONSTRAINT [PK_Incidents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DriverId] in table 'Laps'
ALTER TABLE [dbo].[Laps]
ADD CONSTRAINT [FK_Laps_To_Drivers]
    FOREIGN KEY ([DriverId])
    REFERENCES [dbo].[Drivers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Laps_To_Drivers'
CREATE INDEX [IX_FK_Laps_To_Drivers]
ON [dbo].[Laps]
    ([DriverId]);
GO

-- Creating foreign key on [SessionId] in table 'Laps'
ALTER TABLE [dbo].[Laps]
ADD CONSTRAINT [FK_Laps_To_Sessions]
    FOREIGN KEY ([SessionId])
    REFERENCES [dbo].[Sessions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Laps_To_Sessions'
CREATE INDEX [IX_FK_Laps_To_Sessions]
ON [dbo].[Laps]
    ([SessionId]);
GO

-- Creating foreign key on [SessionId] in table 'Results'
ALTER TABLE [dbo].[Results]
ADD CONSTRAINT [FK_SessionResult]
    FOREIGN KEY ([SessionId])
    REFERENCES [dbo].[Sessions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SessionResult'
CREATE INDEX [IX_FK_SessionResult]
ON [dbo].[Results]
    ([SessionId]);
GO

-- Creating foreign key on [DriverId] in table 'Results'
ALTER TABLE [dbo].[Results]
ADD CONSTRAINT [FK_DriverResult]
    FOREIGN KEY ([DriverId])
    REFERENCES [dbo].[Drivers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DriverResult'
CREATE INDEX [IX_FK_DriverResult]
ON [dbo].[Results]
    ([DriverId]);
GO

-- Creating foreign key on [SessionId] in table 'Incidents'
ALTER TABLE [dbo].[Incidents]
ADD CONSTRAINT [FK_SessionIncidents]
    FOREIGN KEY ([SessionId])
    REFERENCES [dbo].[Sessions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SessionIncidents'
CREATE INDEX [IX_FK_SessionIncidents]
ON [dbo].[Incidents]
    ([SessionId]);
GO

-- Creating foreign key on [DriverId1] in table 'Incidents'
ALTER TABLE [dbo].[Incidents]
ADD CONSTRAINT [FK_DriverIncident1]
    FOREIGN KEY ([DriverId1])
    REFERENCES [dbo].[Drivers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DriverIncident1'
CREATE INDEX [IX_FK_DriverIncident1]
ON [dbo].[Incidents]
    ([DriverId1]);
GO

-- Creating foreign key on [DriverId2] in table 'Incidents'
ALTER TABLE [dbo].[Incidents]
ADD CONSTRAINT [FK_DriverIncidents2]
    FOREIGN KEY ([DriverId2])
    REFERENCES [dbo].[Drivers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DriverIncidents2'
CREATE INDEX [IX_FK_DriverIncidents2]
ON [dbo].[Incidents]
    ([DriverId2]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------