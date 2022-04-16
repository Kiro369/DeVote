// convert int to byte array.
function getByteArrayFromInt(int) {
    const buf = Buffer.allocUnsafe(4);
    buf.writeInt32LE(int, 0);
    // console.log(`bytes array for ${int} are ${buf}`);
    return buf
};

// convert byte array to int.
function getIntFromByteArray(bytes) {
    const buf = Buffer.from(bytes);
    // console.log("buf length", buf.length);
    const int = buf.readInt32LE(0)
    // console.log("int", int);
    return int
};

module.exports = { getByteArrayFromInt, getIntFromByteArray }