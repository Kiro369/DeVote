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
    const { id, name, lat, lng } = req.query;
    console.log({ id, name, lat, lng });

    let insertVM = handleErrors(mySQLite.insertVM, { id, lat, lng, name });
    let isErrorCatched = await catchErrors(insertVM, req, res);

    console.log("isErrorCatched", isErrorCatched)
    if (!isErrorCatched) res.send({ result: { id, lat, lng, name } });
});

module.exports = vmRouter;