const { DBWrapper } = require('./db-utils');
require('dotenv').config();

const SQLITEDB_NAME = process.env.SQLITEDB_NAME;
const LEVELDB_PATH = process.env.LEVELDB_PATH;
const MINS_TO_SYNC_DBS = parseInt(process.env.MINS_TO_SYNC_DBS);
const SYNC_DELAY_TIME = 1000 * 60 * MINS_TO_SYNC_DBS;
global.isDBsOpen = false;

(async () => {

    const myDBWrapper = new DBWrapper();
    await myDBWrapper.initDBs(SQLITEDB_NAME, LEVELDB_PATH)
        .then(async () => {
            // share a single global DB connection with routes.
            global.SQLite = myDBWrapper.mySQLite;
        })
        .catch(err => {
            console.error("Opening Databases failed");
            console.error(err);
            return;
        });

    if (!global.isDBsOpen) return;

    console.log("Opening Databases succeed.");
    console.log(`Syncing Databases every ${MINS_TO_SYNC_DBS} Minutes`)
    console.log("===============================================");


    console.log("App is ready to start.")

    setInterval(async () => {
        console.log("===============================================");
        console.log(`${new Date().toLocaleString()} : Syncing Databases ...`)
        await myDBWrapper.syncDBs()
        console.log("===============================================");

    }, 1000 * 30);
    //SYNC_DELAY_TIME
    const express = require('express');
    const cors = require('cors')
    const blockRouter = require('./routes/blockRouter.js');
    const txRouter = require('./routes/txRouter.js');
    const vmRouter = require('./routes/vmRouter.js');

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
    app.use('/transactions', txRouter);
    app.use('/vms', vmRouter);

    app.use(express.static('api-test'))

    app.listen(app.get('port'), function () {
        console.log(`App started at ${new Date().toLocaleString()}`);
        console.log(`App started on http://localhost:${app.get('port')}`);
    })
})()

