USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderDetail')
BEGIN
CREATE TABLE OrderDetail(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ItemId INT NOT NULL,
    CONSTRAINT FK_OrderDetail_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetail_Order_OrderId FOREIGN KEY (OrderId) REFERENCES [Order](Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('OrderDetail already created :)');