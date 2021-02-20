USE master
GO
IF NOT EXISTS(SELECT *
              FROM sys.databases
              WHERE name = 'brevien-sqldb')
    BEGIN
        CREATE DATABASE [brevien-sqldb]
    END
GO

USE [brevien-sqldb]
GO

-- CREATE MercuryBus schema, tables and indexes
IF NOT EXISTS(SELECT *
              FROM sys.schemas
              WHERE name = 'mercury')
    BEGIN

        EXEC ('CREATE SCHEMA mercury');

        CREATE TABLE mercury.Messages
        (
            Id           VARCHAR(450) PRIMARY KEY,
            Destination  NVARCHAR(1000) NOT NULL,
            Headers      NVARCHAR(1000) NOT NULL,
            Payload      NVARCHAR(MAX)  NOT NULL,
            Published    SMALLINT DEFAULT 0,
            CreationTime BIGINT
        );

        -- ADD default value expression for creation_time
        ALTER TABLE mercury.Messages
            ADD DEFAULT DATEDIFF_BIG(ms, '1970-01-01 00:00:00', GETUTCDATE()) FOR CreationTime

        CREATE TABLE mercury.ReceivedMessages
        (
            ConsumerId   VARCHAR(450),
            MessageId    VARCHAR(450),
            PRIMARY KEY (ConsumerId, MessageId),
            CreationTime BIGINT
        );

        -- ADD default value expression for creation_time
        ALTER TABLE mercury.ReceivedMessages
            ADD DEFAULT DATEDIFF_BIG(ms, '1970-01-01 00:00:00', GETUTCDATE()) FOR CreationTime

        CREATE INDEX Message_Published_IDX ON mercury.Messages (Published, Id);
    END
GO

-- CREATE Posting service schema, tables and indexes
IF NOT EXISTS(SELECT *
              FROM sys.schemas
              WHERE name = 'posting')
    BEGIN

        EXEC ('CREATE SCHEMA posting');

        CREATE TABLE posting.Posts
        (
            Id        NVARCHAR(36)  NOT NULL,
            UserId    NVARCHAR(36)  NOT NULL,
            BlogId    NVARCHAR(36)  NOT NULL,
            Title     NVARCHAR(200) NOT NULL,
            Content   NVARCHAR(max) NOT NULL,
            CreatedAt DATETIME2     NOT NULL,
            UpdatedAt DATETIME2,
            Url       NVARCHAR(100) NOT NULL,
            CONSTRAINT PK_Posts
                PRIMARY KEY (Id)
        )

        CREATE TABLE posting.Comments
        (
            Id        NVARCHAR(36)   NOT NULL,
            UserId    NVARCHAR(36)   NOT NULL,
            Content   NVARCHAR(2500) NOT NULL,
            CreatedAt DATETIME2      NOT NULL,
            PostId    NVARCHAR(36)   NOT NULL,
            UpdatedAt DATETIME2,
            CONSTRAINT PK_Comments
                PRIMARY KEY (Id),
            CONSTRAINT FK_Comments_Posts_PostId
                FOREIGN KEY (PostId)
                    REFERENCES posting.Posts
                    ON DELETE CASCADE
        )

        CREATE TABLE posting.Reactions
        (
            Id        NVARCHAR(36) NOT NULL,
            UserId    NVARCHAR(36) NOT NULL,
            Type      smallint     NOT NULL,
            CreatedAt DATETIME2    NOT NULL,
            PostId    NVARCHAR(36) NOT NULL,
            CONSTRAINT PK_Reactions
                PRIMARY KEY (Id),
            CONSTRAINT FK_Reactions_Posts_PostId
                FOREIGN KEY (PostId)
                    REFERENCES posting.Posts
                    ON DELETE CASCADE
        )

        CREATE TABLE posting.Blogs
        (
            Id   NVARCHAR(36)  NOT NULL,
            Name NVARCHAR(200) NOT NULL,
            Uri  NVARCHAR(100) NOT NULL,
            CONSTRAINT PK_Blogs
                PRIMARY KEY (Id)
        )

        CREATE TABLE posting.BlogOwners
        (
            BlogId NVARCHAR(36) NOT NULL,
            UserId NVARCHAR(36) NOT NULL,
            CONSTRAINT PK_BlogUser
                PRIMARY KEY (BlogId, UserId),
            CONSTRAINT FK_Blogs_Id
                FOREIGN KEY (BlogId)
                    REFERENCES posting.Blogs (Id)
                    ON DELETE CASCADE
        )

        CREATE INDEX IX_Posts_UserId
            on posting.Posts (UserId)

        CREATE INDEX IX_Posts_BlogId
            on posting.Posts (BlogId)

        CREATE INDEX IX_Comments_UserId
            on posting.Comments (UserId)

        CREATE INDEX IX_Comments_PostId
            on posting.Comments (PostId)

        CREATE INDEX IX_Reactions_UserId
            on posting.Reactions (UserId)

        CREATE INDEX IX_Reactions_PostId
            on posting.Reactions (PostId)

        CREATE INDEX IX_BlogUser_BlogId_UserId
            on posting.BlogOwners (BlogId, UserId)
    END
GO
