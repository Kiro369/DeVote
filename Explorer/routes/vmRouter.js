const { Router } = require('express');
const { handleErrors, catchErrors } = require('../middleware/errorRequestHandler.js');

const vmRouter = Router();
const resourceType = "voting machine";
let mySQLite = global.SQLite;

vmRouter.get('/', async (req, res) => {
    const vms = await mySQLite.getVMs();
    res.send({ result: vms });
});

vmRouter.post('/', async (req, res) => {
    console.log(req.body);
    let insertVM = handleErrors(mySQLite.insertVM, req.body);
    let isErrorCatched = await catchErrors(insertVM, req, res);
    console.log("isErrorCatched", isErrorCatched)
    if (!isErrorCatched) res.send({ result: req.body });
});

module.exports = vmRouter;