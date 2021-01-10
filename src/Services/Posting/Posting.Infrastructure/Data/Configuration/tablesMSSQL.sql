CREATE TABLE Posts
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

CREATE TABLE Comments
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
        FOREIGN KEY (PostId) REFERENCES Posts
)

CREATE TABLE Reactions
(
    Id        NVARCHAR(36) NOT NULL,
    UserId    NVARCHAR(36) NOT NULL,
    Type      smallint     NOT NULL,
    CreatedAt DATETIME2    NOT NULL,
    PostId    NVARCHAR(36) NOT NULL,
    CONSTRAINT PK_Reactions
        PRIMARY KEY (Id),
    CONSTRAINT FK_Reactions_Posts_PostId
        FOREIGN KEY (PostId) REFERENCES Posts
)

CREATE INDEX IX_Posts_UserId
    on Posts (UserId)

CREATE INDEX IX_Posts_BlogId
    on Posts (BlogId)

CREATE INDEX IX_Comments_UserId
    on Comments (UserId)

CREATE INDEX IX_Comments_PostId
    on Comments (PostId)

CREATE INDEX IX_Reactions_UserId
    on Reactions (UserId)

CREATE INDEX IX_Reactions_PostId
    on Reactions (PostId)

