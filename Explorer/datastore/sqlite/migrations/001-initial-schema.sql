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
    Date       TEXT,
    Hash       TEXT PRIMARY KEY,
    Elector    TEXT,
    Elected    TEXT,
    BlockHeight INTEGER,
    FOREIGN KEY (BlockHeight) REFERENCES Blocks (Height)
);