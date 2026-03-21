SET NOCOUNT ON;
GO
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

-- Announcement seed
PRINT('Seeding Announcements...')
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Announcements')
BEGIN
    IF (SELECT COUNT(*) FROM Announcements) = 0
    BEGIN
        INSERT INTO Announcements (Message, Color, StartDate, EndDate, IsActive)
        VALUES (
            'Welcome to Room2Room!',
            'warning',
            DATEADD(day,-1, SYSUTCDATETIME()),
            DATEADD(day,7, SYSUTCDATETIME()),
            1
        );
        print('Announcement inserted')
    END
    ELSE
        print('Announcements already seeded :)')
END
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
    INSERT INTO Item (ItemName, ItemDescription, ItemPrice, Status, CategoryId, AccountId, UniversityName, IsDeleted) VALUES
        ('Laptop', '15-inch laptop with 8GB RAM and 256GB SSD', 750, 'Available', 1, 1, 'University of Regina', 0),
        ('Smartphone', 'Android phone, 128GB storage, excellent condition', 400, 'Available', 1, 2, 'University of Regina', 0),
        ('Headphones', 'Noise-cancelling over-ear headphones', 120.5, 'Sold', 1, 3, 'University of Saskatchewan', 0),
        ('Textbook: CS101', 'Introduction to Computer Science textbook, used', 60, 'Available', 2, 1, 'University of Regina', 0),
        ('Textbook: Math201', 'Advanced calculus textbook, like new', 85, 'Available', 2, 2, 'University of Regina', 0),
        ('Desk Chair', 'Ergonomic chair, black color', 95, 'Available', 3, 3, 'University of Saskatchewan', 0),
        ('Dining Table', 'Wooden dining table for 4 people', 200, 'Sold', 3, 4, 'University of Saskatchewan', 0),
        ('Winter Jacket', 'Medium size, waterproof', 65, 'Available', 4, 5, 'University of Nagasaki', 0),
        ('Sneakers', 'Size 10, lightly used', 50, 'Available', 4, 1, 'University of Regina', 0),
        ('Backpack', '25L laptop backpack, brand new', 45, 'Available', 4, 2, 'University of Regina', 0),
        ('Coffee Maker', 'Single-serve coffee machine', 35, 'Sold', 5, 3, 'University of Saskatchewan', 0),
        ('Toaster', 'Countertop toaster with 2 slots', 25, 'Available', 5, 4, 'University of Saskatchewan', 0),
        ('Bicycle', 'Mountain bike, 21-speed', 300, 'Available', 6, 5, 'University of Nagasaki', 0),
        ('Monitor', '24-inch LED monitor', 150, 'Available', 1, 1, 'University of Regina', 0),
        ('USB Flash Drive', '64GB USB 3.0 drive', 15, 'Sold', 1, 2, 'University of Regina', 0),
        ('Notebook', 'Pack of 5 notebooks, college ruled', 10, 'Available', 2, 3, 'University of Saskatchewan', 0),
        ('Bookshelf', '4-tier wooden bookshelf', 80, 'Available', 3, 4, 'University of Saskatchewan', 0),
        ('Hoodie', 'Large size, cotton hoodie', 30, 'Sold', 4, 5, 'University of Nagasaki', 0),
        ('Camera', 'Digital camera with 18-55mm lens', 220, 'Available', 1, 1, 'University of Regina', 0),
        ('Gaming Mouse', 'Wired RGB gaming mouse', 40, 'Available', 1, 2, 'University of Regina', 0)
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

IF (SELECT COUNT(*) FROM Chat) = 0
BEGIN
    PRINT('Inserting Chat')
    SET IDENTITY_INSERT Chat ON;
    INSERT INTO Chat (ChatId, ListingId, ChatType) VALUES
       (1, 1, 'listing'),
       (2, 2, 'listing'),
       (3, 3, 'listing'),
       (4, 3, 'listing'),
       (5, 3, 'listing'),
       (6, 3, 'listing'),
       (7, null, 'private')
    SET IDENTITY_INSERT Chat OFF;
    
    
    PRINT('Inserting ChatMembers')
    INSERT INTO ChatMember (ChatId, AccountId) VALUES
       (1, 1),
       (1, 2),
       (2, 1),
       (2, 2),
       (3, 1),
       (3, 2),
       (4, 1),
       (4, 2),
       (5, 1),
       (5, 2),
       (6, 1),
       (6, 2),
       (7, 1),
       (7, 2)
    
    PRINT('Inserting ChatMessage')
    SET IDENTITY_INSERT ChatMessage ON;
    INSERT INTO ChatMessage ([MessageId],[ChatId],[Message],[CreatedAt],[FromAccountId]) VALUES
       (1, 1, 'Hello there', DATEADD(minute, -10, GETDATE()), 1),
       (2, 1, 'Hi! Is the laptop still available?', DATEADD(minute, -9, GETDATE()), 2),
       (3, 1, 'Yes, it is. Do you have any questions about it?', DATEADD(minute, -8, GETDATE()), 1),
       (4, 1, 'What is the battery life like?', DATEADD(minute, -7, GETDATE()), 2),
       (5, 1, 'It lasts about 6 hours on a full charge.', DATEADD(minute, -6, GETDATE()), 1),
       (6, 1, 'Great! I am interested. Can we meet tomorrow?', DATEADD(minute, -5, GETDATE()), 2),
       (7, 1, 'Tomorrow works for me. What time were you thinking?', DATEADD(minute, -4, GETDATE()), 1),
       (8, 1, 'How about around noon?', DATEADD(minute, -3, GETDATE()), 2),
       (9, 1, 'Noon is perfect. See you then!', DATEADD(minute, -2, GETDATE()), 1),
       (10, 7, 'This is a chat without a listing', DATEADD(minute, -4, GETDATE()), 1),
       (11, 7, 'Woah, how can there be no listing???', DATEADD(minute, -1, GETDATE()), 2)
    SET IDENTITY_INSERT ChatMessage OFF;
END
