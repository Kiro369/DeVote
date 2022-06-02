const { SQLite } = require('../datastore/sqlite/sqlite.js');
const { levelDB } = require('./leveldb.js');
const fs = require("fs");
const { ProtoBuf } = require('../protobuf/protobuf.js');
const path = require('path');
const { _success, _warn, _info, _breaker } = require("../misc/logger.js")

// The maximum positive value for a 32-bit signed integer.
const BLOCK_HEIGHT_LIMIT = 2147483647;

class DBWrapper {

    async initSQliteAndBlksDir(SQLITEDB_NAME, BLKS_PATH, PROTO_FILE_NAME, MINS_TO_SYNC_DBS) {
        console.log("\nOpening Databases...")

        this.mySQLite = new SQLite();
        await this.mySQLite.openSQLiteDB(SQLITEDB_NAME)
        global.isDBsOpen = true;

        if (!fs.existsSync(BLKS_PATH)) throw new Error(`Blocks Directory not found. "${BLKS_PATH}"`);
        _success(`Blocks Directory found.\n`)

        this.protobuf = new ProtoBuf();
        this.deserializer = await this.protobuf.loadProtoFile(PROTO_FILE_NAME);
        global.isProtoFileLoaded = true;

        this.syncTime = MINS_TO_SYNC_DBS;
        this.blkPath = BLKS_PATH;
    }

    async syncSQLWithBlks() {
        if (!fs.existsSync(this.blkPath)) throw new Error(`Blocks Directory not found. "${this.blkPath}"`);

        const blksFiles = fs.readdirSync(this.blkPath)

        const lbh_SQLite = await this.mySQLite.getLatestBlockHeight();
        console.log("latestBlockHeight_SQLite ", lbh_SQLite);

        let lbh_Blks = blksFiles.length;
        console.log("latestBlockHeight_Blks   ", lbh_Blks);

        if (lbh_SQLite == lbh_Blks) {
            _success("\nDatabase Status : SQLiteDB is in sync with Blks Dir")
            this.logSyncTime()
            return;
        }
        if (lbh_SQLite < lbh_Blks) {
            _warn("\nDatabase Status : SQLiteDB is out of sync with Blks Dir.\n")
            let offset = lbh_SQLite + 1;
            let noBlocksToSync = lbh_Blks - lbh_SQLite;

            // Syncing 50 blocks for the initial sync.
            if (lbh_SQLite == 0 && lbh_Blks >= 50) {
                noBlocksToSync = 50;
                lbh_Blks = 50;
            }

            console.log(`Offset: ${offset} | End : ${lbh_Blks}`)
            _info(`Syncing ${noBlocksToSync} Blocks`)


            for (let index = offset; index <= lbh_Blks; index++) {
                const blkFilePath = path.resolve(this.blkPath, `${index}.blk`)
                const blkContent = Buffer.from(fs.readFileSync(blkFilePath))
                let deserializedBlock = this.deserializer.deserializeBlock(blkContent)
                await this.mySQLite.insertBlock(deserializedBlock.toJSON())
            }
            this.logSyncTime()
        }
    }

    async initDBs(SQLiteDB_Name, LevelDB_Name, PROTO_FILE_NAME, MINS_TO_SYNC_DBS) {
        console.log("\nOpening Databases...")

        this.mySQLite = new SQLite();
        await this.mySQLite.openSQLiteDB(SQLiteDB_Name)

        this.protobuf = new ProtoBuf();
        this.deserializer = await this.protobuf.loadProtoFile(PROTO_File_Name);

        this.mylevelDB = new levelDB();
        await this.mylevelDB.openLevelDB(LevelDB_Name);

        await this.mylevelDB.setSerializer(PROTO_FILE_NAME)
            .catch(err => console.error(err))
    }

    async syncDBs() {
        const lbh_SQLite = await this.mySQLite.getLatestBlockHeight();
        console.log("latestBlockHeight_SQLite ", lbh_SQLite);

        const lbh_LevelDB = parseInt(await this.mylevelDB.getLatestBlockHeight_byteArray(lbh_SQLite, BLOCK_HEIGHT_LIMIT));
        console.log("latestBlockHeight_LevelDB", lbh_LevelDB);

        if (lbh_SQLite == lbh_LevelDB) {
            console.log("\nDatabase Status : SQLiteDB is in sync with LevelDB ")
            console.log(`Syncing  Time   : Every ${syncTime} minutes`)
            _breaker(false);
            return;
        }

        if (lbh_SQLite < lbh_LevelDB) {
            _breaker();
            console.log("Database Status : SQLiteDB is out of sync with LevelDB ")
            _breaker();
            const myNewBlocks = await this.mylevelDB.getBlocksFromTo_byteArray(lbh_SQLite, lbh_LevelDB)
            // console.log(myNewBlocks.map(block => block.Height))
            console.log(`Syncing ${myNewBlocks.length} Blocks`)
            await this.mySQLite.insertBlocks(myNewBlocks)
            console.log(`Done inserting ${myNewBlocks.length} Blocks`);

            _breaker();
            this.logSyncTime()
        }
    }

    logSyncTime() {
        _info(`\nSyncing  Time   : Every ${this.syncTime} minutes`)
        _breaker(false);
    }
}

module.exports = { DBWrapper }