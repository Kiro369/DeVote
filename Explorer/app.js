const { DBWrapper } = require('./sync/db-utils.js');
const { _success, _warn, _info, _breaker, _time } = require("./misc/logger.js");
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
            _warn("Opening Databases failed");
            console.error(err);
            return;
        });

    if (!global.isDBsOpen || !isProtoFileLoaded) return;

    _success("\nOpening Databases succeed.");
    _info(`Syncing Databases every ${MINS_TO_SYNC_DBS} minutes\n`)

    _success("App is ready to start.")

    // handle the unhandled 
    process.on("unhandledRejection", error => {
        console.error(error)
    });

    setTimeout(async () => {
        _time("Inserting initial Governorates ...", false)
        await myDBWrapper.mySQLite.insertInitialGovernorates();

        _time("Initializing Databases Syncing ...", false)
        await myDBWrapper.syncSQLWithBlks();

        _time("Setting initial NoVotes Of Candidates ...")
        await myDBWrapper.mySQLite.setNoVotesForCandidates();
    }, 1000 * 5)

    // keep syncing databases then counting and updating votes for candidates and governorates.
    setInterval(async () => {
        _breaker()
        _time("Syncing Databases ...")
        await myDBWrapper.syncSQLWithBlks();

        _time("Updating NoVotes Of Candidates ...")
        await myDBWrapper.mySQLite.setNoVotesForCandidates();

        _time("Updating Votes For Governorates ...", false)
        await myDBWrapper.mySQLite.setNoVotesForGovernorates();
    }, SYNC_DELAY_TIME);

    const express = require('express');
    const cors = require('cors')
    const blockRouter = require('./routes/blockRouter.js');
    const txRouter = require('./routes/txRouter.js');
    const vmRouter = require('./routes/vmRouter.js');
    const candidateRouter = require('./routes/candidateRouter.js');
    const governorateRouter = require('./routes/governorateRouter.js');
    const { lookup } = require('dns').promises;
    const { hostname } = require('os');

    const app = express();
    const port = process.env.PORT || 3000;

    app.set('port', port);
    app.disable('x-powered-by');

    async function getLocalIPAddress() {
        return (await lookup(hostname(), { family: 4 })).address;
    }

    let ip = await getLocalIPAddress()

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

    app.listen(app.get('port'), ip, function () {
        console.log(`App started at ${new Date().toLocaleString()}`);
        _success(`App started on ${ip}:${app.get('port')}`);
        _info(`App initialization process will start in 5s`);
        _breaker(false);
    })
})()

