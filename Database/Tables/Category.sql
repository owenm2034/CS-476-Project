USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Category')
BEGIN
CREATE TABLE Category(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(40) NOT NULL
);
END
ELSE
    PRINT('Category already created :)');