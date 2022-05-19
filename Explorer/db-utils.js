const { SQLite } = require('./datastore/sqlite/sqlite.js');
const { levelDB } = require('./leveldb.js');

// The maximum positive value for a 32-bit signed integer.
const BLOCK_HEIGHT_LIMIT = 2147483647;

class DBWrapper {
    async initDBs(SQLiteDB_Name, LevelDB_Name, PROTO_File_Name) {
        console.log("\nOpening Databases...")

        this.mySQLite = new SQLite();
        await this.mySQLite.openSQLiteDB(SQLiteDB_Name)

        this.mylevelDB = new levelDB();
        await this.mylevelDB.openLevelDB(LevelDB_Name);

        await this.mylevelDB.setSerializer(PROTO_File_Name)
            .catch(err => console.error(err))
    }

    async syncDBs(MINS_TO_SYNC_DBS) {
        const lbh_SQLite = await this.mySQLite.getLatestBlockHeight();
        console.log("latestBlockHeight_SQLite ", lbh_SQLite);

        const lbh_LevelDB = parseInt(await this.mylevelDB.getLatestBlockHeight_byteArray(lbh_SQLite, BLOCK_HEIGHT_LIMIT));
        console.log("latestBlockHeight_LevelDB", lbh_LevelDB);

        if (lbh_SQLite == lbh_LevelDB) {
            console.log("\nDatabase Status : SQLiteDB is in sync with LevelDB ")
            console.log(`Syncing  Time   : Every ${MINS_TO_SYNC_DBS} minutes`)
            console.log("=====================================================\n");
            return;
        }

        if (lbh_SQLite < lbh_LevelDB) {
            console.log("=======================================================");
            console.log("Database Status : SQLiteDB is out of sync with LevelDB ")
            console.log("=======================================================");
            const myNewBlocks = await this.mylevelDB.getBlocksFromTo_byteArray(lbh_SQLite, lbh_LevelDB)
            // console.log(myNewBlocks.map(block => block.Height))
            console.log(`Syncing ${myNewBlocks.length} Blocks`)
            await this.mySQLite.insertBlocks(myNewBlocks)
            console.log(`Done inserting ${myNewBlocks.length} Blocks`);

            console.log("=======================================================");
            console.log(`Syncing  Time   : Every ${MINS_TO_SYNC_DBS} minutes`)
            console.log("=======================================================\n");
        }

    }
}

module.exports = { DBWrapper }