USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaveLater')
BEGIN
CREATE TABLE SaveLater(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(MAX) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
END
ELSE
    PRINT('SaveLater already created :)');