CREATE TABLE  Blocks (
    Height      INTEGER UNIQUE,
    PrevHash    TEXT,
    Timestamp   INTEGER,
    MerkleRoot  TEXT,
    Hash        TEXT PRIMARY KEY,
    Miner       TEXT,
    nTx         INTEGER
);

CREATE TABLE Transactions (
    Date  INTEGER,
    Hash       TEXT PRIMARY KEY,
    Elector    TEXT,
    Elected    TEXT,
    BlockHeight INTEGER,
    FOREIGN KEY (BlockHeight) REFERENCES Blocks (Height)
);

CREATE TABLE VMachines (
    ID        TEXT PRIMARY KEY,
    Name      TEXT NOT NULL UNIQUE,
    Lat       INTEGER NOT NULL,
    Lng       INTEGER NOT NULL
);