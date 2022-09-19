async function cursorBasedPaginationHandler(req, res, resourceType) {
    const mySQLite = global.SQLite;

    const cursorParamName = resourceType == "block" ? "heightCursor" : "idCursor";
    const cursorColumnName = resourceType == "block" ? "Height" : "ID";

    const limit = parseInt(req.query["limit"]) || 20;
    if (limit > 50) limit = 50;

    const lbh = await mySQLite.getLatestBlockHeight();
    const ltt = await mySQLite.getLatestTxId();

    const obh = await mySQLite.getOldestBlockHeight();
    const ott = await mySQLite.getOldestTxId();

    // console.log("ltt", ltt)
    // console.log("ott", ott)

    const maxCursorValue = resourceType == "block" ? lbh : ltt;
    const minCursorValue = resourceType == "block" ? obh : ott;

    const cursorParam = req.query[cursorParamName];
    let operator = " < ";
    let order = "DESC";
    let cursor;
    let paginationDir;

    if (cursorParam) {
        paginationDir = cursorParam.split("_")[0]
        operator = paginationDir == "next" ? " > " : " < ";
        order = paginationDir == "next" ? " ASC " : " DESC ";
        cursor = parseInt(cursorParam.split("_")[1])
    }


    // if (!cursor) cursor = maxCursorValue;
    let more = true;
    let prev, next;
    let pagination = {};

    // case one : initial request by the client
    // if (cursor == maxCursorValue) {
    if (!cursor) {
        console.log("Case 1 : Initial request by the client")
        // increment by 1 to avoid comparison by equality in SQL query.
        // set cursor to the max value of the unique column in the initial request.
        cursor = maxCursorValue;
        cursor += 1;
        next = null;
    }

    // other cases : subsequent requests
    if (cursor <= maxCursorValue) {
        console.log("Case 2 : Subsequent requests by client")
    }

    console.log(`cursor : ${cursor}  limit : ${limit}  operator : ${operator} `)

    let items = resourceType == "block" ?
        await mySQLite.getBlocksByHeightCursorAndLimit(cursor, limit, operator, order) :
        await mySQLite.getTxsByIdCursorAndLimit(cursor, limit, operator, order);


    if (paginationDir == "next") items = items.reverse();

    if (!items.length) more = false;

    if (items.length) {
        prev = items[items.length - 1][cursorColumnName]
        next = items[0][cursorColumnName]
    }

    // console.log("prev", prev)
    // console.log("minCursorValue", minCursorValue);
    // console.log("limit", limit)
    // console.log("cursor", cursor)

    const host = req.headers.host;
    let protocol = req.protocol || "https"
    // if (host.includes("localhost")) protocol = "http"

    // console.log("host", host)
    // console.log("req.secure ", req.secure)
    // console.log("req.protocol ", req.protocol)

    if (prev == minCursorValue) more = false;
    if (prev != minCursorValue) pagination["prev"] = `${protocol}://${host}/${resourceType}s?limit=${limit}&${cursorParamName}=prev_${prev}`;
    if (next != maxCursorValue) pagination["next"] = `${protocol}://${host}/${resourceType}s?limit=${limit}&${cursorParamName}=next_${next}`;

    pagination["more"] = more;
    pagination["max"] = maxCursorValue;


    res.send({ pagination, result: items })
}

module.exports = { cursorBasedPaginationHandler };