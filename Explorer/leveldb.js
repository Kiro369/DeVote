const { Level } = require('level');

class levelDB {
    async openLevelDB(path) {
        this.db = new Level(path);

        this.db.open({ createIfMissing: false }, (err) => {
            if (err) console.log("Connection to LevelDB failed, database doesn't exist.")
            if (!err) console.log("Connection to LevelDB is successful")
        })

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
        const Blocks = []
        for await (const [key, value] of this.db.iterator({ gt: start, limit: start - end })) {
            const block = JSON.parse(value)
            Blocks.push(block)
        }
        return Blocks;
    }

}

module.exports = { levelDB }