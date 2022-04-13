const { open } = require('sqlite')
const sqlite3 = require('sqlite3')
const path = require('path')

class SQLite {

    async openSQLiteDB(dbName) {
        this.db = await open({
            filename: path.join(__dirname, `${dbName}.sqlite`),
            driver: sqlite3.Database
        }).catch(err => console.log(err));

        await this.db.migrate({
            migrationsPath: path.join(__dirname, 'migrations'),
        }).then(() => console.log("Connection to SQLiteDB is successful"))

        return this
    };

    async closeSQLiteDB() {
        this.db.close((err) => {
            if (err) {
                console.error(err.message);
            }
            console.log('Close the database connection.');
        });
    };

    async getLatestBlockHeight() {
        const lbh = await this.db.get("SELECT MAX(Height) FROM Blocks;")
        return lbh["MAX(Height)"] || 0
    };

    async getBlocksByHeightCursorAndLimit(heightCursor, limit) {
        const blocks = await this.db.all(`
        SELECT * FROM Blocks
        WHERE Height < ?
        ORDER BY Height DESC
        LIMIT ?;`, heightCursor, limit);
        return blocks
    }

    async insertBlock(block) {
        const { Height, PrevHash, Timestamp, MerkleRoot, Hash, Miner, nTx } = block;
        await this.db.run(
            "INSERT INTO Blocks VALUES (?,?,?,?,?,?,?)",
            Height, PrevHash, Timestamp, MerkleRoot, Hash, Miner, nTx
        );

        const { Transactions } = block;
        await this.insertTxs(Transactions, Height)
    };

    async insertBlocks(blocks) {
        for (let index = 0; index < blocks.length; index++) {
            const block = blocks[index];
            await this.insertBlock(block)
        }
    };

    async getBlockByHeight(height) {
        let targetBlock = await this.db.get("SELECT * FROM Blocks WHERE Height == ?", height)
        return targetBlock
    };

    async getBlockByHash(hash) {
        let targetBlock = await this.db.get("SELECT * FROM Blocks WHERE Hash == ?", hash)
        return targetBlock
    };

    async getLatestTxTimestamp() {
        const ltt = await this.db.get("SELECT MAX(Date) FROM Transactions")
        return ltt["MAX(Date)"] || 0
    };

    async getOldestTxTimestamp() {
        const ltt = await this.db.get("SELECT MIN(Date) FROM Transactions")
        return ltt["MIN(Date)"] || 0
    };

    async getTxsByTimestampCursorAndLimit(timestampCursor, limit, operator, order) {
        const SQL_QUERY = `
        SELECT * FROM Transactions
        WHERE Date ${operator} ?
		ORDER BY Date ${order}
        LIMIT ?`
        const txs = await this.db.all(SQL_QUERY, timestampCursor, limit);
        return txs
    }

    async insertTx(tx, blockHeight) {
        const { Hash, Elector, Elected } = tx;
        let date = new Date(tx["Date"]).getTime();
        this.db.run('PRAGMA foreign_keys = OFF;');
        await this.db.run("INSERT INTO Transactions VALUES (?,?,?,?,?)",
            date, Hash, Elector, Elected, parseInt(blockHeight)
        )
        this.db.run('PRAGMA foreign_keys = ON;');
    };

    async insertTxs(txs, blockHeight) {
        for (let index = 0; index < txs.length; index++) {
            const tx = txs[index];
            await this.insertTx(tx, blockHeight)
        }
    };

    async getTxByHash(hash) {
        let targetTx = await this.db.get("SELECT * FROM Transactions WHERE Hash == ?", hash)
        return targetTx
    };

    async getTxsByBlockHeight(blockHeight) {
        let targetTxs = await this.db.all("SELECT * FROM Transactions WHERE BlockHeight == ?", blockHeight)
        return targetTxs
    };

    async insertVM(vm) {
        console.log("vm", vm)
        const { id, lat, lng, name } = vm;
        await this.db.run(
            "INSERT INTO VMachines  VALUES (?,?,?,?)",
            id, name, lat, lng
        )
    };

    async getVMs() {
        let vms = await this.db.all("SELECT * FROM VMachines")
        if (!vms.length) throw new Error("No VMs Found")
        return vms
    }
}

module.exports = { SQLite }