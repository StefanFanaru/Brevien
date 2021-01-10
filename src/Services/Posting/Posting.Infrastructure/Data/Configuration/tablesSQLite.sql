CREATE TABLE Posts
(
    Id        TEXT NOT NULL,
    UserId    TEXT NOT NULL,
    BlogId    TEXT NOT NULL,
    Title     TEXT NOT NULL,
    Content   TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NULL,
    Url       TEXT NOT NULL,
    CONSTRAINT PK_Posts
        PRIMARY KEY (Id)
);

CREATE TABLE Comments
(
    Id        TEXT NOT NULL,
    UserId    TEXT NOT NULL,
    Content   TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    PostId    TEXT NOT NULL,
    UpdatedAt TEXT,
    CONSTRAINT PK_Comments
        PRIMARY KEY (Id),
    CONSTRAINT FK_Comments_Posts_PostId
        FOREIGN KEY (PostId) REFERENCES Posts
);

CREATE TABLE Reactions
(
    Id        TEXT    NOT NULL,
    UserId    TEXT    NOT NULL,
    Type      INTEGER NOT NULL,
    CreatedAt TEXT    NOT NULL,
    PostId    TEXT    NOT NULL,
    CONSTRAINT PK_Reactions
        PRIMARY KEY (Id),
    CONSTRAINT FK_Reactions_Posts_PostId
        FOREIGN KEY (PostId) REFERENCES Posts
);

CREATE INDEX IX_Posts_UserId
    on Posts (UserId);

CREATE INDEX IX_Posts_BlogId
    on Posts (BlogId);

CREATE INDEX IX_Comments_UserId
    on Comments (UserId);

CREATE INDEX IX_Comments_PostId
    on Comments (PostId);

CREATE INDEX IX_Reactions_UserId
    on Reactions (UserId);

CREATE INDEX IX_Reactions_PostId
    on Reactions (PostId);

