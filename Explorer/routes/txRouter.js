const { Router } = require('express');
const { notFoundErrorHandler } = require('../middleware/errorRequestHandler.js');
const { cursorBasedPaginationHandler } = require('../middleware/paginationHandler.js');

const txRouter = Router();
const resourceType = "transaction";
const mySQLite = global.SQLite;

txRouter.get('/', async (req, res) => {
    cursorBasedPaginationHandler(req, res, "transaction")
});

txRouter.get('/tx-hash/:txHash', async (req, res) => {
    const { txHash } = req.params;
    const tx = await mySQLite.getTxByHash(txHash);
    if (!tx) notFoundErrorHandler(req, res, resourceType, "hash", txHash);
    if (tx) res.send({ result: tx });
});

txRouter.get('/tx-block-height/:BlockHeight', async (req, res) => {
    const { BlockHeight } = req.params;
    const txs = await mySQLite.getTxsByBlockHeight(BlockHeight);
    if (!txs.length) notFoundErrorHandler(req, res, resourceType, "blockHeight", BlockHeight);
    if (txs.length) res.send({ result: txs });
});

module.exports = txRouter;