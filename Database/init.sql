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


IF (SELECT COUNT(*) FROM Category) = 0
BEGIN
    PRINT('Inserting 5 Categories...')
    INSERT INTO Category (CategoryName) VALUES
        ('Electronics'),
        ('Academic Supplies'),
        ('Furniture'),
        ('Clothing'),
        ('Kitchen Appliances'),
        ('Outdoor & Sports');
END
ELSE
    PRINT('Category already seeded :)')

IF (SELECT COUNT(*) FROM Item) = 0
BEGIN
    PRINT('Inserting 20 Items...')
    INSERT INTO Item (ItemName, ItemDescription, ItemPrice, Status, CategoryId, AccountId) VALUES
        ('Laptop', '15-inch laptop with 8GB RAM and 256GB SSD', 750, 'Available', 1, 1),
        ('Smartphone', 'Android phone, 128GB storage, excellent condition', 400, 'Available', 1, 2),
        ('Headphones', 'Noise-cancelling over-ear headphones', 120.5, 'Sold', 1, 3),
        ('Textbook: CS101', 'Introduction to Computer Science textbook, used', 60, 'Available', 2, 1),
        ('Textbook: Math201', 'Advanced calculus textbook, like new', 85, 'Available', 2, 2),
        ('Desk Chair', 'Ergonomic chair, black color', 95, 'Available', 3, 3),
        ('Dining Table', 'Wooden dining table for 4 people', 200, 'Sold', 3, 4),
        ('Winter Jacket', 'Medium size, waterproof', 65, 'Available', 4, 5),
        ('Sneakers', 'Size 10, lightly used', 50, 'Available', 4, 1),
        ('Backpack', '25L laptop backpack, brand new', 45, 'Available', 4, 2),
        ('Coffee Maker', 'Single-serve coffee machine', 35, 'Sold', 5, 3),
        ('Toaster', 'Countertop toaster with 2 slots', 25, 'Available', 5, 4),
        ('Bicycle', 'Mountain bike, 21-speed', 300, 'Available', 6, 5),
        ('Monitor', '24-inch LED monitor', 150, 'Available', 1, 1),
        ('USB Flash Drive', '64GB USB 3.0 drive', 15, 'Sold', 1, 2),
        ('Notebook', 'Pack of 5 notebooks, college ruled', 10, 'Available', 2, 3),
        ('Bookshelf', '4-tier wooden bookshelf', 80, 'Available', 3, 4),
        ('Hoodie', 'Large size, cotton hoodie', 30, 'Sold', 4, 5),
        ('Camera', 'Digital camera with 18-55mm lens', 220, 'Available', 1, 1),
        ('Gaming Mouse', 'Wired RGB gaming mouse', 40, 'Available', 1, 2)
END
ELSE
    PRINT('Items already seeded :)')

IF (SELECT COUNT(*) FROM ItemImage) = 0
BEGIN
    PRINT('Inserting 20 ItemImages...')
    INSERT INTO ItemImage (ImagePath, ItemId) VALUES
        ('/images/laptop.jpg', 1),
        ('/images/smartphone.jpg', 2),
        ('/images/headphones.jpg', 3),
        ('/images/textbook1.jpg', 4),
        ('/images/textbook2.jpg', 5),
        ('/images/desk-chair.jpg', 6),
        ('/images/dining-table.jpg', 7),
        ('/images/winter-jacket.jpg', 8),
        ('/images/sneakers.jpg', 9),
        ('/images/comingSoon.jpg', 10),
        ('/images/comingSoon.jpg', 11),
        ('/images/comingSoon.jpg', 12),
        ('/images/comingSoon.jpg', 13),
        ('/images/comingSoon.jpg', 14),
        ('/images/comingSoon.jpg', 15),
        ('/images/comingSoon.jpg', 16),
        ('/images/comingSoon.jpg', 17),
        ('/images/comingSoon.jpg', 18),
        ('/images/comingSoon.jpg', 19),
        ('/images/comingSoon.jpg', 20)
END
ELSE
    PRINT('ItemImage already seeded :)')
