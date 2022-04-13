const { Router } = require('express');
const { notFoundErrorHandler } = require('../middleware/errorRequestHandler.js');
const { cursorBasedPaginationHandler } = require('../middleware/paginationHandler.js');

const blockRouter = Router();
const resourceType = "block";
const mySQLite = global.SQLite;

blockRouter.get('/', async (req, res) => {
    cursorBasedPaginationHandler(req, res, "block")
});

blockRouter.get('/block-hash/:blockHash', async (req, res) => {
    const { blockHash } = req.params;
    const block = await mySQLite.getBlockByHash(blockHash);
    if (!block) notFoundErrorHandler(req, res, resourceType, "hash", blockHash);
    if (block) res.send({ result: block })
});

blockRouter.get('/block-height/:blockHeight', async (req, res) => {
    const { blockHeight } = req.params;
    const block = await mySQLite.getBlockByHeight(blockHeight);
    if (!block) notFoundErrorHandler(req, res, resourceType, "height", blockHeight);
    if (block) res.send({ result: block })
});

module.exports = blockRouter;