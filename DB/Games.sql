CREATE TABLE [dbo].[Games]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newsequentialid(), 
    [Field] VARCHAR(7) NOT NULL, 
    [Mines] VARCHAR(MAX) NOT NULL, 
    [Moves] VARCHAR(MAX) NOT NULL, 
    [Result] CHAR NULL
)
