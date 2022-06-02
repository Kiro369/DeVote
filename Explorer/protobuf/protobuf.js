const protobuf = require("protobufjs");
const path = require("path");
const { _success, _warn, _info } = require("../misc/logger")

class ProtoBuf {

    async loadProtoFile(protofileName) {
        const fileName = `${protofileName}.proto`
        const dir = path.join(__dirname, fileName);

        console.log(`Loading ${fileName}`)

        this.root = await protobuf.load(dir)
            .catch(err => { throw err; })

        this.message = this.root.lookupType(`${protofileName}Package.Block`);

        _success(`Loading ${fileName} succeed.`)
        return this
    }

    deserializeBlock(blockBuf) {
        const block = this.message.decode(blockBuf);
        return block
    }
}

module.exports = { ProtoBuf }