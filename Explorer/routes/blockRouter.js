const { Router } = require('express');
const { SQLite } = require('../datastore/sqlite/sqlite.js');

const blockRouter = Router();
let mySQLite;

(async () => {
    mySQLite = new SQLite();
    await mySQLite.openSQLiteDB("deVote");
})();

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