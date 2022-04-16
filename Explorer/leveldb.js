const { Level } = require('level');
const path = require('path');
const fs = require('fs');
const { getByteArrayFromInt, getIntFromByteArray } = require("./protobuf/buffer-utils.js");
const { ProtoBuf } = require('./protobuf/protobuf.js');

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
        console.log(`Connection to ${dbPath} LevelDB  is successful.`)

        return this;
    }

    async setSerializer(protofileName) {
        this.protobuf = new ProtoBuf();
        this.serializer = await this.protobuf.loadProtoFile(protofileName);
        global.isProtoFileLoaded = true;
    }

    async getLatestBlockHeight_string(start, limit) {
        // console.log(start, limit);
        const keys = await this.db.keys({ gt: start - 1, limit: limit }).all()
        // console.log(keys);
        if (!keys.length) return start;
        if (keys.length) return Math.max(...keys.map(key => parseInt(key)))
    }

    async getLatestBlockHeight_byteArray(start, limit) {
        console.log(`Start : ${start} - Limit : ${limit}`);

        const bufStart = getByteArrayFromInt(start);
        const keys = await this.db.keys({ keyEncoding: "binary", gt: bufStart, limit: limit }).all();

        // the start is the maximum height that currently exists in leveldb.
        if (!keys.length) return start;

        // console.log(`keys ${keys}`);

        let intKeys = keys.map(key => getIntFromByteArray(key))
        // console.log(`intKeys ${intKeys}`);


        const max = Math.max(...intKeys)
        // console.log(`max ${max}`);

        if (keys.length) return max
    }

    async getAllBlocks_string() {
        const Blocks = []
        for await (const [key, value] of this.db.iterator()) {
            const block = JSON.parse(value)
            Blocks.push(block)
        }
        return Blocks;
    }

    async getAllBlocks_byteArray() {
        const Blocks = []
        for await (const [key, value] of this.db.iterator({ valueEncoding: 'binary' })) {
            let deserializedBlock = this.serializer.deserializeBlock(Buffer.from(value))
            console.log("deserializedBlock", deserializedBlock)
            Blocks.push(deserializedBlock.toJSON())
        }
        return Blocks;
    }

    async getBlocksFromTo_string(start, end) {
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

    async getBlocksFromTo_byteArray(start, end) {
        const bufStart = getByteArrayFromInt(start);

        let limit = end - start;
        // Syncing 50 blocks for the initial sync.
        if (start == 0) limit = 50;
        console.log(`start : ${start} end : ${end} limit : ${limit} `);

        const Blocks = []
        for await (const [key, value] of this.db.iterator({ keyEncoding: 'binary', valueEncoding: 'binary', gt: bufStart, limit: limit })) {
            let deserializedBlock = this.serializer.deserializeBlock(Buffer.from(value))
            // console.log("deserializedBlock", deserializedBlock)
            Blocks.push(deserializedBlock.toJSON())
        }
        console.log(`Returned Blocks length : ${Blocks.length}`);
        return Blocks;
    }

}

module.exports = { levelDB }