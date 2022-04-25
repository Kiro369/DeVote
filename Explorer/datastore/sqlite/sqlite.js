const { open } = require('sqlite')
const sqlite3 = require('sqlite3')
const path = require('path')

class SQLite {

    async openSQLiteDB(dbName) {
        this.db = await open({
            filename: path.join(__dirname, `${dbName}.sqlite`),
            driver: sqlite3.Database
        }).catch(err => { throw err; })

        await this.db.migrate({
            migrationsPath: path.join(__dirname, 'migrations'),
        }).catch(err => { throw err; })

        console.log(`Connection to ${dbName} SQLiteDB is successful.`)
        return this
    };

    async closeSQLiteDB() {
        this.db.close()
            .then(() => console.log('Connection to SQLiteDB is closed.'))
            .catch(err => { throw err; })
    };

    async getLatestBlockHeight() {
        const lbh = await this.db.get("SELECT MAX(Height) FROM Blocks;")
        return lbh["MAX(Height)"] || 0
    };

    async getOldestBlockHeight() {
        const lbh = await this.db.get("SELECT MIN(Height) FROM Blocks;")
        return lbh["MIN(Height)"] || 0
    };

    async getBlocksByHeightCursorAndLimit(heightCursor, limit, operator, order) {
        const SQL_QUERY = `
        SELECT * FROM Blocks
        WHERE Height ${operator} ?
		ORDER BY Height ${order}
        LIMIT ?`
        const blocks = await this.db.all(SQL_QUERY, heightCursor, limit);
        return blocks
    }

    async insertBlock(block) {
        const { Height, PrevHash, Timestamp, MerkleRoot, Hash, Miner, nTx } = block;
        console.log(`Inserting Block : Hash ${Hash} -  Height ${Height}`)
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
        let { Hash, Date, Elector, Elected, Confirmations } = tx;
        if (!Confirmations) Confirmations = 0;
        this.db.run('PRAGMA foreign_keys = OFF;');
        await this.db.run("INSERT INTO Transactions VALUES (?,?,?,?,?,?)",
            Date, Hash, Elector, Elected, Confirmations, parseInt(blockHeight)
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
        const { id, lat, lng } = vm;
        await this.db.run(
            "INSERT INTO VMachines  VALUES (?,?,?)",
            id, lat, lng
        )
    };

    async getVMs() {
        let vms = await this.db.all("SELECT * FROM VMachines")
        // if (!vms.length) throw new Error("No VMs Found")
        return vms
    }
}

module.exports = { SQLite }