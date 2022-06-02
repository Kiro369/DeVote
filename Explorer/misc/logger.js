function _success(msg) {
    console.log("\x1b[32m%s\x1b[0m", msg)
}

function _warn(msg) {
    console.log("\x1b[31m%s\x1b[0m", msg)
}

function _info(msg) {
    console.log("\x1b[36m%s\x1b[0m", msg)
}

function _breaker(in_line = true) {
    if (in_line) console.log("===============================================");
    else console.log("===============================================\n")
}

function _time(msg, in_line = true) {
    let time = new Date().toLocaleString();
    if (in_line) console.log(`${time} : ${msg}...`);
    else console.log(`\n${time} : ${msg}...`);
}

module.exports = { _success, _warn, _info, _breaker, _time }