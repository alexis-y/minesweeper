CREATE TABLE [dbo].[Games]
(
    [Id] UNIQUEIDENTIFIER   NOT NULL PRIMARY KEY DEFAULT newsequentialid(),
    [OwnerId] NVARCHAR(450) NULL,
    [Field] VARCHAR(7)      NOT NULL, 
    [Mines] VARCHAR(MAX)    NOT NULL, 
    [Moves] VARCHAR(MAX)    NOT NULL, 
    [Flags] VARCHAR(MAX)    NOT NULL,
    [Result]                CHAR NULL
)
