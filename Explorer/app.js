const { DBWrapper } = require('./db-utils');
require('dotenv').config();

const SQLITEDB_NAME = process.env.SQLITEDB_NAME;
const LEVELDB_NAME = process.env.LEVELDB_NAME;
const PROTO_FILE_NAME = process.env.PROTO_FILE_NAME;
const MINS_TO_SYNC_DBS = parseFloat(process.env.MINS_TO_SYNC_DBS);
const SYNC_DELAY_TIME = 1000 * 60 * MINS_TO_SYNC_DBS;
global.defaultTieColor = parseInt("0xffb4b4b4");

global.isDBsOpen = false;
global.isProtoFileLoaded = false;

(async () => {

    const myDBWrapper = new DBWrapper();
    await myDBWrapper.initDBs(SQLITEDB_NAME, LEVELDB_NAME, PROTO_FILE_NAME)
        .then(async () => {
            // share a single global DB connection with routes.
            global.SQLite = myDBWrapper.mySQLite;
        })
        .catch(err => {
            console.error("Opening Databases failed");
            console.error(err);
            return;
        });

    if (!global.isDBsOpen || !isProtoFileLoaded) return;

    console.log("Opening Databases succeed.");
    console.log(`Syncing Databases every ${MINS_TO_SYNC_DBS} minutes\n`)

    console.log("App is ready to start.")

    // handle the unhandled 
    process.on("unhandledRejection", error => {
        console.error(error)
    });

    setTimeout(async () => {
        console.log(`${new Date().toLocaleString()} : Initializing Databases Syncing ...`)
        await myDBWrapper.syncDBs(MINS_TO_SYNC_DBS)

        console.log(`${new Date().toLocaleString()} : Setting initial NoVotes Of Candidates ...`)
        await myDBWrapper.mySQLite.setNoVotesForCandidates();

        console.log(`\n${new Date().toLocaleString()} : Inserting initial Governorates ...`)
        await myDBWrapper.mySQLite.insertInitialGovernorates();
    }, 1000 * 5)

    // keep syncing databases then counting and updating votes for candidates and governorates.
    setInterval(async () => {
        console.log("\n===============================================");
        console.log(`${new Date().toLocaleString()} : Syncing Databases ...`)
        await myDBWrapper.syncDBs(MINS_TO_SYNC_DBS);

        console.log(`${new Date().toLocaleString()} : Upadating NoVotes Of Candidates ...`)
        await myDBWrapper.mySQLite.setNoVotesForCandidates();

        console.log(`\n${new Date().toLocaleString()} : Upadating Votes For Governorates ...`)
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
        console.log(`App initialization process will start in 5s`);
        console.log("===============================================\n");
    })
})()

