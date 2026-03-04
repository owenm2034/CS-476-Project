USE Room2Room;
GO

SET ANSI_NULLS OFF
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NotificationPreferences')
BEGIN
CREATE TABLE NotificationPreferences(
    AccountId INT PRIMARY KEY,
    RecieveEmailNotificationOnChatMessageRecieved BIT NOT NULL,
    RecieveEmailNotificationOnUserReported BIT NOT NULL,
    RecieveEmailNotificationOnListingReported BIT NOT NULL,
    CONSTRAINT FK_NotificationPreference_Account_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
);
END
ELSE
    PRINT('NotificationPreferences already created :)');