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

IF NOT EXISTS (select * from sys.tables where name = 'Listings')
BEGIN
CREATE TABLE Listings(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    ItemDescription NVARCHAR(500) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Condition NVARCHAR(100) NOT NULL,
    ItemStatus NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    
    AccountId INT NOT NULL,
    UniversityId INT NOT NULL,
    -- does not allow foreign key constraints

);
print ('Listings created')
END
ELSE
    print('Listings already created :)')


-- DATA SEEDING
Print('*** BEGIN DATA SEEDING ***')

PRINT('Inserting 5 Accounts...')
INSERT INTO Accounts (Email, PasswordHash, PasswordSalt, Username, IsEmailVerified, IsAdmin, ProfilePictureUrl, UniversityId) VALUES
    ('476user@uregina.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'ureginauser',	1,	0,	'',	2195),
    ('476admin@uregina.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'ureginaadmin',	1,	1,	'',	2195),
    ('476user@usask.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'usaskuser',	1,	0,	'',	2196),
    ('476admin@usask.ca',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'usaskadmin',	1,	1,	'',	2196),
    ('476other@nagasaki-u.ac.jp',	'yNpN/VbLcE73n/RZggWQGll2aeR3R2n9OXzOrB57jd4=',	'7PYJYar8UGDwaPUCx4+r/Q==',	'unagasakiuser',	1,	0,	'',	5117)

PRINT('Inserting 5 Items...')
INSERT INTO Listings (Title, ItemDescription, Category, Condition, ItemStatus, Price, CreatedAt, UpdatedAt, AccountId, UniversityId) VALUES
    ('Dell Laptop', 'A lightly used Dell XPS 13, 8GB RAM, 256GB SSD.', 'Electronics', 'Used', 'Available', 750.00, GETDATE(), GETDATE(), 1, 2195),
    ('Wooden Desk', 'Solid oak desk, perfect for study or work.', 'Furniture', 'New', 'Available', 200.00, GETDATE(), GETDATE(), 2, 2195),
    ('Calculus Textbook', 'Calculus 10th Edition, good condition, minor notes inside.', 'Books', 'Used', 'Sold', 50.00, GETDATE(), GETDATE(), 3, 2196),
    ('Mountain Bike', '26-inch mountain bike, lightly used, great for trails.', 'Sports', 'Used', 'Available', 300.00, GETDATE(), GETDATE(), 1, 2195),
    ('iPhone 12', 'Apple iPhone 12, 64GB, black, works perfectly.', 'Electronics', 'Used', 'Available', 500.00, GETDATE(), GETDATE(), 4, 5117);


