USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaveLaterDetail')
BEGIN
CREATE TABLE SaveLaterDetail(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SaveLaterId INT NOT NULL,
    ItemId INT NOT NULL,
    CONSTRAINT FK_SaveLaterDetail_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SaveLaterDetail_SaveLater_SaveLaterId FOREIGN KEY (SaveLaterId) REFERENCES SaveLater(Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('SaveLaterDetail already created :)');