CREATE TABLE  Blocks (
    Height          INTEGER UNIQUE,
    PrevHash        TEXT,
    Timestamp       INTEGER,
    MerkleRoot      TEXT,
    Hash            TEXT PRIMARY KEY,
    Miner           TEXT,
    nTx             INTEGER
);

CREATE TABLE Transactions (
    ID              INTEGER PRIMARY KEY AUTOINCREMENT,
    Date            INTEGER,
    Hash            TEXT,
    Elector         TEXT,
    Elected         TEXT,
    Confirmations   INTEGER NOT NULL,
    BlockHeight     INTEGER,
    FOREIGN KEY (BlockHeight) REFERENCES Blocks (Height)
);

CREATE TABLE VMachines (
    ID              TEXT PRIMARY KEY,
    Lat             INTEGER NOT NULL,
    Lng             INTEGER NOT NULL,
    Address         TEXT NOT NULL,
    Governorate     Text NOT NULL
);

CREATE TABLE Candidates (
    ID              TEXT PRIMARY KEY,
    Name            Text NOT NULL,
    NoVotes         INTEGER NOT NULL,
    Color           INTEGER NOT NULL
);

CREATE TABLE Governorates (
    ID              INTEGER PRIMARY KEY AUTOINCREMENT,
    ArabicName      Text NOT NULL,
    EnglishName     Text NOT NULL,
    IDsOfVMs        Text,
    Votes           Text NOT NULL,
    Color           INTEGER NOT NULL
);