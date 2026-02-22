USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Item')
BEGIN
CREATE TABLE Item(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(40) NOT NULL,
    ItemDescription NVARCHAR(200) NOT NULL,
    ItemPrice FLOAT NOT NULL,
    Status NVARCHAR(MAX) NOT NULL,
    CategoryId INT NOT NULL,
    AccountId INT NOT NULL,
    UniversityId INT NOT NULL,
    CONSTRAINT FK_Item_University_UniversityId FOREIGN KEY (UniversityId) REFERENCES Universities(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Item_Accounts_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Item_Category_CategoryId FOREIGN KEY (CategoryId) REFERENCES Category(Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('Item already created :)');