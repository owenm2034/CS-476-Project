USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Watchlist')
BEGIN
CREATE TABLE Watchlist(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ItemId INT NOT NULL,
    DateAdded DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Watchlist_Accounts_UserId FOREIGN KEY (UserId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Watchlist_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE NO ACTION
);
print ('Watchlist created')
END
ELSE
    PRINT('Watchlist already created :)');