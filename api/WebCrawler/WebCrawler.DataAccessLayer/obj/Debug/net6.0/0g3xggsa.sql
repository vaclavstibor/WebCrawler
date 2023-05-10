IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Records] (
    [Id] int NOT NULL IDENTITY,
    [URL] nvarchar(max) NOT NULL,
    [RegExp] nvarchar(max) NOT NULL,
    [Periodicity] datetime2 NOT NULL,
    [Label] nvarchar(max) NOT NULL,
    [Active] bit NOT NULL,
    CONSTRAINT [PK_Records] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220408185128_Initial', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [WebsiteRecordId] int NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tags_Records_WebsiteRecordId] FOREIGN KEY ([WebsiteRecordId]) REFERENCES [Records] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Tags_WebsiteRecordId] ON [Tags] ([WebsiteRecordId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220408185413_add-tags', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220417143604_nodes', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'Periodicity');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Records] DROP COLUMN [Periodicity];
GO

ALTER TABLE [Records] ADD [Days] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Records] ADD [Hours] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Records] ADD [Minutes] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220417160419_time-update', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'RegExp');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Records] ALTER COLUMN [RegExp] nvarchar(max) NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'Minutes');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Records] ALTER COLUMN [Minutes] int NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'Label');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Records] ALTER COLUMN [Label] nvarchar(max) NULL;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'Hours');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Records] ALTER COLUMN [Hours] int NULL;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'Days');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Records] ALTER COLUMN [Days] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220607110641_update', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Records] ADD [ExecutionStatus] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Records] ADD [LastExecution] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220607162323_changeTagsTypeInRecord', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'LastExecution');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Records] ALTER COLUMN [LastExecution] datetime2 NULL;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Records]') AND [c].[name] = N'ExecutionStatus');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Records] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Records] ALTER COLUMN [ExecutionStatus] bit NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220607165224_nullable', N'6.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Records] ADD [StartingNodeId] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [Nodes] (
    [Id] int NOT NULL IDENTITY,
    [Domain] nvarchar(max) NOT NULL,
    [Group] int NOT NULL,
    [NodeId] int NULL,
    CONSTRAINT [PK_Nodes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Nodes_Nodes_NodeId] FOREIGN KEY ([NodeId]) REFERENCES [Nodes] ([Id])
);
GO

CREATE INDEX [IX_Records_StartingNodeId] ON [Records] ([StartingNodeId]);
GO

CREATE INDEX [IX_Nodes_NodeId] ON [Nodes] ([NodeId]);
GO

ALTER TABLE [Records] ADD CONSTRAINT [FK_Records_Nodes_StartingNodeId] FOREIGN KEY ([StartingNodeId]) REFERENCES [Nodes] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220608174852_nodess', N'6.0.3');
GO

COMMIT;
GO

