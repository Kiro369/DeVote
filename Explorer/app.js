const { DBWrapper } = require('./db-utils');
require('dotenv').config();

const SQLITEDB_NAME = process.env.SQLITEDB_NAME;
const LEVELDB_NAME = process.env.LEVELDB_NAME;
const PROTO_FILE_NAME = process.env.PROTO_FILE_NAME;
const MINS_TO_SYNC_DBS = parseFloat(process.env.MINS_TO_SYNC_DBS);
const SYNC_DELAY_TIME = 1000 * 60 * MINS_TO_SYNC_DBS;
const BLKS_PATH = process.env.BLKS_PATH;

global.defaultTieColor = parseInt("0xffb4b4b4");
global.isDBsOpen = false;
global.isProtoFileLoaded = false;

(async () => {

    const myDBWrapper = new DBWrapper();
    await myDBWrapper.initSQliteAndBlksDir(SQLITEDB_NAME, BLKS_PATH, PROTO_FILE_NAME, MINS_TO_SYNC_DBS)
        .then(async () => {
            // share a single global DB connection with routes.
            global.SQLite = myDBWrapper.mySQLite;
        })
        .catch(err => {
            console.error("\x1b[31m%s\x1b[0m", "Opening Databases failed");
            console.error(err);
            return;
        });

    if (!global.isDBsOpen || !isProtoFileLoaded) return;

    console.log("\x1b[32m%s\x1b[0m", "\nOpening Databases succeed.");
    console.log('\x1b[36m%s\x1b[0m', `Syncing Databases every ${MINS_TO_SYNC_DBS} minutes\n`)

    console.log("\x1b[32m%s\x1b[0m", "App is ready to start.")

    // handle the unhandled 
    process.on("unhandledRejection", error => {
        console.error(error)
    });

    setTimeout(async () => {
        console.log(`\n${new Date().toLocaleString()} : Inserting initial Governorates ...`)
        await myDBWrapper.mySQLite.insertInitialGovernorates();

        console.log(`\n${new Date().toLocaleString()} : Initializing Databases Syncing ...`)
        await myDBWrapper.syncSQLWithBlks();

        console.log(`${new Date().toLocaleString()} : Setting initial NoVotes Of Candidates ...`)
        await myDBWrapper.mySQLite.setNoVotesForCandidates();


    }, 1000 * 5)

    // keep syncing databases then counting and updating votes for candidates and governorates.
    setInterval(async () => {
        console.log("\n===============================================");
        console.log(`${new Date().toLocaleString()} : Syncing Databases ...`)
        await myDBWrapper.syncSQLWithBlks();

        console.log(`${new Date().toLocaleString()} : Updating NoVotes Of Candidates ...`)
        await myDBWrapper.mySQLite.setNoVotesForCandidates();

        console.log(`\n${new Date().toLocaleString()} : Updating Votes For Governorates ...`)
        await myDBWrapper.mySQLite.setNoVotesForGovernorates();
    }, SYNC_DELAY_TIME);

    const express = require('express');
    const cors = require('cors')
    const blockRouter = require('./routes/blockRouter.js');
    const txRouter = require('./routes/txRouter.js');
    const vmRouter = require('./routes/vmRouter.js');
    const candidateRouter = require('./routes/candidateRouter.js');
    const governorateRouter = require('./routes/governorateRouter.js');

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
    app.use('/candidates', candidateRouter);
    app.use('/governorates', governorateRouter);

    app.use(express.static('api-test'))

    app.listen(app.get('port'), function () {
        console.log(`App started at ${new Date().toLocaleString()}`);
        console.log(`App started on http://localhost:${app.get('port')}`);
        console.log("\x1b[36m%s\x1b[0m", `App initialization process will start in 5s`);
        console.log("===============================================\n");
    })
})()

