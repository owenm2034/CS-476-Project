-- DB CREATION
USE master;
GO
IF DB_ID('Room2Room') IS NULL
  CREATE DATABASE Room2Room;
  print("Database Room2Room created.")
GO
USE Room2Room;
GO

SET NOCOUNT ON;
GO

-- TABLE CREATION
Print('*** BEGIN Table Creation ***')

IF NOT EXISTS (select * from sys.tables where name = 'Universities')
BEGIN
CREATE TABLE Universities(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Domain NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL
);
print ('Universities created')
END
ELSE
    print('Universities already created :)')
    

IF NOT EXISTS (select * from sys.tables where name = 'Accounts')
BEGIN
CREATE TABLE Accounts(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(100) NOT NULL,
    PasswordSalt NVARCHAR(100) NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    IsAdmin BIT NOT NULL DEFAULT 0,
    ProfilePictureUrl NVARCHAR(200) NOT NULL,
    UniversityId INT NOT NULL
    CONSTRAINT FK_Accounts_Universities_UniversityId FOREIGN KEY (UniversityId) REFERENCES Universities(Id) ON DELETE CASCADE
);
print ('Accounts created')
END
ELSE
    print('Accounts already created :)')


-- Announcements
IF NOT EXISTS (select * from sys.tables where name = 'Announcements')
BEGIN
CREATE TABLE Announcements(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Message NVARCHAR(1000) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
print ('Announcements created')
END
ELSE
    print('Announcements already created :)')


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Category')
BEGIN
CREATE TABLE Category(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(40) NOT NULL
);
print ('Category created')
END
ELSE
    PRINT('Category already created :)');


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Item')
BEGIN
CREATE TABLE Item(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(40) NOT NULL,
    ItemDescription NVARCHAR(200) NOT NULL,
    ItemPrice FLOAT NOT NULL,
    Status NVARCHAR(MAX) NOT NULL,
    CategoryId INT NOT NULL,
    AccountId INT NOT NULL,
    UniversityName NVARCHAR(100),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Item_Accounts_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Item_Category_CategoryId FOREIGN KEY (CategoryId) REFERENCES Category(Id) ON DELETE CASCADE
);
print ('Item created')
END
ELSE
    PRINT('Item already created :)');

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ItemReport')
BEGIN
CREATE TABLE ItemReport(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ItemId INT NOT NULL,
    ReportedByAccountId INT NOT NULL,
    Reason NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Resolution NVARCHAR(50) NULL,
    ResolvedAt DATETIME2 NULL,
    CONSTRAINT FK_ItemReport_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ItemReport_Accounts_AccountId FOREIGN KEY (ReportedByAccountId) REFERENCES Accounts(Id) ON DELETE NO ACTION
);
print ('ItemReport created')
END
ELSE
    PRINT('ItemReport already created :)');

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Watchlist')
BEGIN
CREATE TABLE Watchlist(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ItemId INT NOT NULL,
    DateAdded DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Watchlist_Accounts_UserId FOREIGN KEY (UserId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Watchlist_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE NO ACTION
);
print ('Watchlist created')
END
ELSE
    PRINT('Watchlist already created :)');


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ItemImage')
BEGIN
CREATE TABLE ItemImage(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ImagePath NVARCHAR(MAX) NOT NULL,
    ItemId INT NOT NULL,
    CONSTRAINT FK_ItemImage_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE
);
print ('ItemImage created')
END
ELSE
    PRINT('ItemImage already created :)');
    
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NotificationPreferences')
BEGIN
CREATE TABLE NotificationPreferences(
    AccountId INT PRIMARY KEY,
    RecieveEmailNotificationOnChatMessageRecieved BIT NOT NULL,
    RecieveEmailNotificationOnUserReported BIT NOT NULL,
    RecieveEmailNotificationOnListingReported BIT NOT NULL,
    CONSTRAINT FK_NotificationPreference_Account_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
);
print ('NotificationPreferences created')
END
ELSE
    PRINT('NotificationPreferences already created :)');


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AccountRestrictions')
BEGIN
CREATE TABLE AccountRestrictions(
    AccountId INT PRIMARY KEY,
    Status NVARCHAR(20) NOT NULL,
    Reason NVARCHAR(255) NULL,
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AccountRestrictions_Accounts_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
);
print ('AccountRestrictions created')
END
ELSE
    PRINT('AccountRestrictions already created :)');



IF NOT EXISTS (select * from sys.tables where name = 'Chat')
BEGIN
CREATE TABLE [Chat] (
  [ChatId] integer PRIMARY KEY IDENTITY(1,1),
  [ListingId] integer,
  [ChatType] VARCHAR(20) NOT NULL
)
print ('Chat created')
END
GO

IF NOT EXISTS (select * from sys.tables where name = 'ChatMessage')
BEGIN
CREATE TABLE [ChatMessage] (
  [MessageId] int PRIMARY KEY IDENTITY(1,1),
  [ChatId] integer NOT NULL,
  [Message] nvarchar(255) NOT NULL,
  [CreatedAt] datetime NOT NULL,
  [FromAccountId] int NOT NULL
)
ALTER TABLE [ChatMessage] ADD FOREIGN KEY ([ChatId]) REFERENCES [Chat] ([ChatId])
print ('ChatMessage created')
END
GO

IF NOT EXISTS (select * from sys.tables where name = 'ChatMember')
BEGIN
CREATE TABLE [ChatMember] (
  [ChatMemberId] integer IDENTITY(1,1) PRIMARY KEY,
  [ChatId] integer NOT NULL,
  [AccountId] integer NOT NULL
)
ALTER TABLE [ChatMember] ADD FOREIGN KEY ([ChatId]) REFERENCES [Chat] ([ChatId])
print ('ChatMember created')
END
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
print ('UserNotification created')
END
ELSE
    PRINT('UserNotification already created :)');
