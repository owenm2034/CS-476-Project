IF NOT EXISTS (select * from sys.tables where name = 'Chat')
BEGIN
CREATE TABLE [Chat] (
  [ChatId] integer PRIMARY KEY IDENTITY(1,1),
  [ListingId] integer,
  [ChatType] VARCHAR(20) NOT NULL
)
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
ALTER TABLE [ChatMessage] ADD FOREIGN KEY ([ChatId]) REFERENCES [Chat] ([ChatId]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (select * from sys.tables where name = 'ChatMember')
BEGIN
CREATE TABLE [ChatMember] (
  [ChatMemberId] integer IDENTITY(1,1) PRIMARY KEY,
  [ChatId] integer NOT NULL,
  [AccountId] integer NOT NULL
)
ALTER TABLE [ChatMember] ADD FOREIGN KEY ([ChatId]) REFERENCES [Chat] ([ChatId]) ON DELETE CASCADE
END
GO

