USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderStatus')
BEGIN
CREATE TABLE OrderStatus(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StatusId INT NOT NULL,
    StatusName NVARCHAR(20) NOT NULL
);
END
ELSE
    PRINT('OrderStatus already created :)');
GO