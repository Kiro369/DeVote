const protobuf = require("protobufjs");
const path = require("path");

class ProtoBuf {

    async loadProtoFile(protofileName) {
        const fileName = `${protofileName}.proto`
        const dir = path.join(__dirname, fileName);

        console.log(`Loading ${fileName}`)

        this.root = await protobuf.load(dir)
            .catch(err => { throw err; })

        this.message = this.root.lookupType(`${protofileName}Package.Block`);

        console.log("\x1b[32m%s\x1b[0m", `Loading ${fileName} succeed.`)
        return this
    }

    deserializeBlock(blockBuf) {
        const block = this.message.decode(blockBuf);
        return block
    }
}

module.exports = { ProtoBuf }