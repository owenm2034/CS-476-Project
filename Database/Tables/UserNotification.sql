USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (select * from sys.tables where name = 'UserNotification')
BEGIN
CREATE TABLE UserNotification(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ItemId INT NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_UserNotification_Accounts_UserId FOREIGN KEY (UserId) REFERENCES Accounts(Id),
    CONSTRAINT FK_UserNotification_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id)
);
END
ELSE
    print('UserNotification already created :)')