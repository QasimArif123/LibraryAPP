USE Libaray;
GO

IF OBJECT_ID('dbo.Members', 'U') IS NOT NULL
    DROP TABLE dbo.Members;
GO

CREATE TABLE dbo.Members (
    MemberId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL
);
GO