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