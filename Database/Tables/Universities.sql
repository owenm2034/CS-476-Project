USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (select * from sys.tables where name = 'Universities')
BEGIN
CREATE TABLE Universities(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Domain NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL
);
END
ELSE
    print('Universities already created :)')