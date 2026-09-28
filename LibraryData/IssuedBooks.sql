USE Libaray;
GO

IF OBJECT_ID('dbo.IssuedBooks', 'U') IS NOT NULL
    DROP TABLE dbo.IssuedBooks;
GO

CREATE TABLE dbo.IssuedBooks (
    IssueId INT IDENTITY(1,1) PRIMARY KEY,
    BookId INT NOT NULL,
    MemberId INT NOT NULL,
    IssueDate DATE NOT NULL,
    ReturnDate DATE NULL,
    FOREIGN KEY (BookId) REFERENCES dbo.Books(BookId),
    FOREIGN KEY (MemberId) REFERENCES dbo.Members(MemberId)
);
GO