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
);
print ('Accounts created')
END
ELSE
    print('Accounts already created :)')


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


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderStatus')
BEGIN
CREATE TABLE OrderStatus(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StatusId INT NOT NULL,
    StatusName NVARCHAR(20) NOT NULL
);
print ('OrderStatus created')
END
ELSE
    PRINT('OrderStatus already created :)');


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShoppingCart')
BEGIN
CREATE TABLE ShoppingCart(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(MAX) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
print ('ShoppingCart created')
END
ELSE
    PRINT('ShoppingCart already created :)');


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
    CONSTRAINT FK_Item_Accounts_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Item_Category_CategoryId FOREIGN KEY (CategoryId) REFERENCES Category(Id) ON DELETE CASCADE
);
print ('Item created')
END
ELSE
    PRINT('Item already created :)');


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
print ('Order created')
END
ELSE
    PRINT('Order already created :)');


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CartDetail')
BEGIN
CREATE TABLE CartDetail(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShoppingCartId INT NOT NULL,
    ItemId INT NOT NULL,
    CONSTRAINT FK_CartDetail_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartDetail_ShoppingCart_ShoppingCartId FOREIGN KEY (ShoppingCartId) REFERENCES ShoppingCart(Id) ON DELETE CASCADE
);
print ('CartDetail created')
END
ELSE
    PRINT('CartDetail already created :)');


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


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderDetail')
BEGIN
CREATE TABLE OrderDetail(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ItemId INT NOT NULL,
    CONSTRAINT FK_OrderDetail_Item_ItemId FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetail_Order_OrderId FOREIGN KEY (OrderId) REFERENCES [Order](Id) ON DELETE CASCADE
);
print ('OrderDetail created')
END
ELSE
    PRINT('OrderDetail already created :)');
    

-- DATA SEEDING
Print('*** BEGIN DATA SEEDING ***')

IF (SELECT COUNT(*) FROM Accounts) = 0
BEGIN
    PRINT('Inserting 5 Accounts...')
    INSERT INTO Accounts (Email, PasswordHash, PasswordSalt, Username, IsEmailVerified, IsAdmin, ProfilePictureUrl, UniversityId) VALUES
        ('476user@uregina.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'ureginauser',	1,	0,	'',	2195),
        ('476admin@uregina.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'ureginaadmin',	1,	1,	'',	2195),
        ('476user@usask.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'usaskuser',	1,	0,	'',	2196),
        ('476admin@usask.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'usaskadmin',	1,	1,	'',	2196),
        ('476other@nagasaki-u.ac.jp',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'unagasakiuser',	1,	0,	'',	5117)
END
ELSE
    PRINT('Accounts already seeded :)')

