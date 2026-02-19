USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (select * from sys.tables where name = 'Listings')
BEGIN
CREATE TABLE Listings(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    ItemDescription NVARCHAR(500) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Condition NVARCHAR(100) NOT NULL,
    ItemStatus NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    
    AccountId INT NOT NULL,
    UniversityId INT NOT NULL,
    -- does not allow foreign key constraints

);
print ('Listings created')
END
ELSE
    print('Listings already created :)')