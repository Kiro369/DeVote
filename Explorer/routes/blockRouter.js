const { Router } = require('express');
const { SQLite } = require('../datastore/sqlite/sqlite.js');

const blockRouter = Router();
let mySQLite;

(async () => {
    mySQLite = new SQLite();
    await mySQLite.openSQLiteDB("deVote");
})();

blockRouter.get('/', async (req, res) => {
    const limit = parseInt(req.query["limit"]) || 20;
    if (limit > 50) limit = 50;

    const lbh = await mySQLite.getLatestBlockHeight();
    let heightCursor = parseInt(req.query["heightCursor"]) || lbh;

    let more = true;
    let prev, next;
    let pagination = {};

    // case one : intial request by the client
    if (heightCursor == lbh) {
        console.log("case 1")
        // increment by 1 to avoid comparison by equality in SQL query
        heightCursor += 1;
        next = null;
    }

    // other cases : subsequent requests
    if (heightCursor < lbh) {
        console.log("case 2")
        next = heightCursor + limit;
    }

    let blocks = await mySQLite.getBlocksByHeightCursorAndLimit(heightCursor, limit)
    prev = blocks[blocks.length - 1]["Height"]
    //console.log(prev)
    if (prev == 1) more = false;
    if (prev != 1) pagination["prev"] = prev;
    if (limit + heightCursor > lbh) next = lbh;
    if (heightCursor - 1 != lbh) pagination["next"] = next;

    pagination["more"] = more;
    pagination["total"] = lbh;

    res.send({ pagination, result: blocks })
});

blockRouter.get('/block-hash/:blockHash', async (req, res) => {
    const { blockHash } = req.params;
    const block = await mySQLite.getBlockByHash(blockHash) || `No Block found for Hash : ${blockHash} `
    res.send({ result: block })
});

blockRouter.get('/block-height/:blockHeight', async (req, res) => {
    const { blockHeight } = req.params;
    const block = await mySQLite.getBlockByHeight(blockHeight) || `No Block found for Height ${blockHeight}`
    res.send({ result: block })
});



module.exports = blockRouter;