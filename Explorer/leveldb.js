const { Level } = require('level');
const path = require('path');
const fs = require('fs');

class levelDB {
    async openLevelDB(dbPath) {
        const dir = path.join(__dirname, dbPath);

        // check if LevelDB directory exists, since leveldb Create new database even if createIfMissing set to false 
        if (!fs.existsSync(dir)) throw new Error(`LevelDB Directory not found. "${dir}"`);

        this.db = new Level(dir);

        this.db.open({ createIfMissing: false }, (err) => {
            if (err) throw new Error("Connection to LevelDB failed.");
        });

        global.isDBsOpen = true;
        console.log("Connection to LevelDB  is successful.")

        return this;
    }

    async getLatestBlockHeight(start, limit) {
        // console.log(start, limit);
        const keys = await this.db.keys({ gt: start - 1, limit: limit }).all()
        // console.log(keys);
        if (!keys.length) return start;
        if (keys.length) return Math.max(...keys.map(key => parseInt(key)))
    }


    async getAllBlocks() {
        const Blocks = []
        for await (const [key, value] of this.db.iterator()) {
            const block = JSON.parse(value)
            Blocks.push(block)
        }
        return Blocks;
    }

    async getBlocksFromTo(start, end) {
        let limit = end - start;
        if (start == 0) limit = end;
        console.log(`start : ${start} end : ${end} limit : ${limit} `)

        const Blocks = []
        for await (const [key, value] of this.db.iterator({ gt: start, limit: limit })) {
            const block = JSON.parse(value)
            Blocks.push(block)
        }
        return Blocks;
    }

}

module.exports = { levelDB }