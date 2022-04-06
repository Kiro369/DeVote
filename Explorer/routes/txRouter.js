const { Router } = require('express');
const { SQLite } = require('../datastore/sqlite/sqlite.js');

const txRouter = Router();
let mySQLite;

(async () => {
    mySQLite = new SQLite();
    await mySQLite.openSQLiteDB("deVote");
})();

txRouter.get('/tx-hash/:txHash', async (req, res) => {
    const { txHash } = req.params;
    const tx = await mySQLite.getTxByHash(txHash) || `No Tx found for Hash ${txHash}`
    res.send({ result: tx })
});

txRouter.get('/tx-block-height/:BlockHeight', async (req, res) => {
    const { BlockHeight } = req.params;
    const txs = await mySQLite.getTxsByBlockHeight(BlockHeight) || `No Txs found for Block Height ${BlockHeight}`
    res.send({ result: txs })
});

module.exports = txRouter;