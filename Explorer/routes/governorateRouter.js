const { Router } = require('express');

const governorateRouter = Router();
const resourceType = "governorate";
let mySQLite = global.SQLite;

governorateRouter.get('/', async (req, res) => {
    const governorates = await mySQLite.getGovernorates_API();
    res.json({ result: governorates });
});

module.exports = governorateRouter;