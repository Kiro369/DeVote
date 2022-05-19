const { Router } = require('express');
const { handleErrors, catchErrors } = require('../middleware/errorRequestHandler.js');

const candidateRouter = Router();
const resourceType = "candidate";
let mySQLite = global.SQLite;

candidateRouter.get('/', async (req, res) => {
    const candidates = await mySQLite.getCandidates();
    res.send({ result: candidates });
});

candidateRouter.post('/', async (req, res) => {
    let { id, name } = req.body;
    console.log(req.body);

    let insertCandidate = handleErrors(mySQLite.insertCandidate, { id, name });
    let isErrorCatched = await catchErrors(insertCandidate, req, res);

    console.log("isErrorCatched", isErrorCatched)
    if (!isErrorCatched) res.send({ result: { id, name } });
});

module.exports = candidateRouter;