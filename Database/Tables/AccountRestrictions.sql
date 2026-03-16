USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AccountRestrictions')
BEGIN
CREATE TABLE AccountRestrictions(
    AccountId INT PRIMARY KEY,
    Status NVARCHAR(20) NOT NULL,
    Reason NVARCHAR(255) NULL,
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AccountRestrictions_Accounts_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('AccountRestrictions already created :)');