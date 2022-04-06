const { DBWrapper } = require('./db-utils');
require('dotenv').config();

const SQLITEDB_NAME = process.env.SQLITEDB_NAME;
const LEVELDB_PATH = process.env.LEVELDB_PATH;


(async () => {

    const myDBWrapper = new DBWrapper();
    await myDBWrapper.initDBs(SQLITEDB_NAME, LEVELDB_PATH)

    setInterval(async () => {
        await myDBWrapper.syncDBs()
    }, 1000 * 30);

    const Blocks = await myDBWrapper.mylevelDB.getAllBlocks();
    // console.log(Blocks);

    let block1 = await myDBWrapper.mySQLite.getBlockByHeight(1);
    // console.log(block1)

    const express = require('express');
    const cors = require('cors')
    const blockRouter = require('./routes/blockRouter.js');
    const txRouter = require('./routes/txRouter.js');

    const app = express();
    const port = process.env.PORT || 3000;

    app.set('port', port);
    app.disable('x-powered-by');

    app.use(cors())
    app.use(express.json())

    app.use(function (req, res, next) {
        console.log('%s %s', req.method, req.url)
        next()
    });

    app.get('/', (req, res) => {
        res.send({ msg: "deVote BackEnd" })
    });

    app.use('/blocks', blockRouter);
    app.use('/txs', txRouter);

    app.listen(app.get('port'), function () {
        console.log(`App started on http://localhost:${app.get('port')}`);
    })
})()

