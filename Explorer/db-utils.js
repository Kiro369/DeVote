const { SQLite } = require('./datastore/sqlite/sqlite.js');
const { levelDB } = require('./leveldb.js');

const BLOCK_HEIGHT_LIMIT = 10 ** 10;

class DBWrapper {
    async initDBs(SQLiteDB_Name, LevelDB_Path) {
        console.log("Opening Databases...")

        this.mySQLite = new SQLite();
        await this.mySQLite.openSQLiteDB(SQLiteDB_Name)

        this.mylevelDB = new levelDB();
        await this.mylevelDB.openLevelDB(LevelDB_Path)
    }

    async syncDBs() {
        const lbh_SQLite = await this.mySQLite.getLatestBlockHeight();
        console.log("latestBlockHeight_SQLite ", lbh_SQLite);

        const lbh_LevelDB = parseInt(await this.mylevelDB.getLatestBlockHeight(lbh_SQLite, BLOCK_HEIGHT_LIMIT));
        console.log("latestBlockHeight_LevelDB", lbh_LevelDB);

        if (lbh_SQLite == lbh_LevelDB) {
            console.log("SQLiteDB is in sync with LevelDB ")
            return;
        }

        if (lbh_SQLite < lbh_LevelDB) {
            console.log("SQLiteDB is out of sync with LevelDB ")
            const myNewBlocks = await this.mylevelDB.getBlocksFromTo(lbh_SQLite, lbh_LevelDB)
            // console.log(myNewBlocks.length)
            // console.log(myNewBlocks.map(block => block.Height))
            console.log(`Syncing ${myNewBlocks.length} Blocks`)
            await this.mySQLite.insertBlocks(myNewBlocks)
        }
    }
}

module.exports = { DBWrapper }