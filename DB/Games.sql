CREATE TABLE [dbo].[Games]
(
    [Id] UNIQUEIDENTIFIER       NOT NULL PRIMARY KEY DEFAULT newsequentialid(),
    [OwnerId] NVARCHAR(450)     NULL,
    [StartTime] DATETIMEOFFSET  NOT NULL,
    [Moves] INT                 NOT NULL,
    [Mines] VARCHAR(MAX)        NOT NULL, 
    [FieldState] VARCHAR(MAX)   NOT NULL,
    [Result]                    INT NULL
)
