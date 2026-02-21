USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Order')
BEGIN
CREATE TABLE [Order](
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(MAX) NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    OrderStatusId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Order_OrderStatus_OrderStatusId FOREIGN KEY (OrderStatusId) REFERENCES OrderStatus(Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('Order already created :)');