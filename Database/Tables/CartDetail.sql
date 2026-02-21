USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CartDetail')
BEGIN
CREATE TABLE CartDetail(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShoppingCartId INT NOT NULL,
    ItemId INT NOT NULL,
    CONSTRAINT FK_CartDetail_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartDetail_ShoppingCart_ShoppingCartId FOREIGN KEY (ShoppingCartId) REFERENCES ShoppingCart(Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('CartDetail already created :)');