# deVote Explorer 
deVote Explorer interfaces with a blockchain node to first extract all the levelDB data. It then stores it in a SQLite database to present the data in a searchable format.

# Using the deVote Explorer API
Query JSON data for blocks and transactions.
## /blocks Endpoints

### /blocks/block-height/:blockHeight : Get block metadata by block height.
```js
/blocks/block-height/50
```
```json
{
  "result": {
    "Height": 50,
    "PrevHash": "2B3CA86D514B5A27D4C1D2365E04A1A7D59E14CFCDF8935677738E301BF692F2",
    "Timestamp": 1649069215442,
    "MerkleRoot": "514E4FC4D7BF898C4694EF88F99EE88069778D307032097AD4112ED62849F1DD",
    "Hash": "81D10DABE0F6C51D097DAFA7675473DFC1370621D13C46274706CC457629CFAE",
    "Miner": "Test33",
    "nTx": 4
  }
}
```
### /blocks/block-hash/:blockHash : Get block metadata by block hash.
```js
/blocks/block-hash/81D10DABE0F6C51D097DAFA7675473DFC1370621D13C46274706CC457629CFAE
```
```json
{
  "result": {
    "Height": 50,
    "PrevHash": "2B3CA86D514B5A27D4C1D2365E04A1A7D59E14CFCDF8935677738E301BF692F2",
    "Timestamp": 1649069215442,
    "MerkleRoot": "514E4FC4D7BF898C4694EF88F99EE88069778D307032097AD4112ED62849F1DD",
    "Hash": "81D10DABE0F6C51D097DAFA7675473DFC1370621D13C46274706CC457629CFAE",
    "Miner": "Test33",
    "nTx": 4
  }
}
```
### /blocks : Get subset of blocks per request.

```js
/blocks/?limit=10&heightCursor=40
```

- `int limit` - The maximum number of blocks to fetch.
 The default limit value is 20.
 The maximum limit value is 50.

- `int heightCursor` - The cursor serving as a pointer to a specific block,  where last request left off.

We are using the unique, sequential column : **blockHeight**.

**Note that the client is responsible for sending back the last block height it fetched to be used as the starting point to fetch the next set of blocks.**

#### How to use /blocks Endpoint:
##### **1- On the initial request, you don't pass a heightCursor parameter :**
```js
/blocks/?limit=20
```
you also may not pass a limit parameter and the default value retrieves the latest 20 blocks.

**The server's response to the *initial request* will include a pagination object that includes a prev property when there are additional blocks to be retrieved.**

**Note that you have to save prev value.**

**Response :**
```json
{
  "pagination": {
    "prev": 31,
    "more": true,
    "total": 50
  },
  "result": [
    {
      "Height": 50,
      "PrevHash": "2B3CA86D514B5A27D4C1D2365E04A1A7D59E14CFCDF8935677738E301BF692F2",
      "Timestamp": 1649069215442,
      "MerkleRoot": "514E4FC4D7BF898C4694EF88F99EE88069778D307032097AD4112ED62849F1DD",
      "Hash": "81D10DABE0F6C51D097DAFA7675473DFC1370621D13C46274706CC457629CFAE",
      "Miner": "Test33",
      "nTx": 4
    },
    ...
    {
      "Height": 31,
      "PrevHash": "3404A2ECA5E5BD60A770EDAC80920E4D1572E0A436C471E004555652842AF3AF",
      "Timestamp": 1649069214300,
      "MerkleRoot": "CCC2F3778B3A4116BE79B5B1634E3DF516E022A2BE41A99F8FFB63FA5F85BB5D",
      "Hash": "18C329DE5A0EDD5A47143F3965F8B206BE87669818D6CCC4878E53845E49BEB5",
      "Miner": "Test33",
      "nTx": 4
    }
  ]
}
```

##### **2- On subsequent requests, you have to pass a heightCursor parameter :**
```js
/blocks/?limit=20&heightCursor=31
```
**Response :**
```json
{
  "pagination": {
    "prev": 11,
    "next": 50,
    "more": true,
    "total": 50
  },
  "result": [
    {
      "Height": 30,
      "PrevHash": "651474BCC0D390A7343BC57318F8447D5285FE2E3AD3D67A728DB634DCE0167A",
      "Timestamp": 1649069214254,
      "MerkleRoot": "38576957B576E5AD36DD60CE11F34E565B79E437E1F8F243769B2D947173DE6B",
      "Hash": "3404A2ECA5E5BD60A770EDAC80920E4D1572E0A436C471E004555652842AF3AF",
      "Miner": "Test33",
      "nTx": 4
    },
    ...
    {
      "Height": 11,
      "PrevHash": "2593071FAE0F68DD714FACB5DE8C54E1FE7FF73F96F967CB418FC7F514205115",
      "Timestamp": 1649069213262,
      "MerkleRoot": "E5193DD7E3F1355811584FFE754BC2F3A222D19D58ED1783714A21AD645E2963",
      "Hash": "E2E03051CBE99F2013FA2850B9149ABED8DB89DA34442960E2ADB9590AC10315",
      "Miner": "Test33",
      "nTx": 4
    }
  ]
}
```
**Note that the pagination's object includes a ***next*** property, which points to block height of the next page.**

**On your next request, you can either fetch prev or next set of blocks**

**For example, to retrieve the next ***older*** set of the blocks [Block 10 : Block 1] .** 
- Set the heightCursor parameter to the *prev* value you received on the last request.
```js
/blocks/?limit=20&heightCursor=11
```

**to retrieve the next ***newer*** set of the blocks [Block 50 : Block 31].**
- Set the heightCursor parameter to the *next* value you received on the last request.
```js
/blocks/?limit=20&heightCursor=50
```
</br>
</br>

## /txs Endpoints
### /txs/tx-hash/:txHash : Get single tx by tx hash.
```js
/txs/tx-hash/913756634CDB1FEF198557F83FC976D8D54960B539603D3CC6BE9E4B2B5AF299
```
```json
{
  "result": {
    "Date": "2022-04-04T10:46:49.8465773Z",
    "Hash": "4E2CF16853AFCAD5E881480D1E86E971ABC6BE1BEA17E9CFBED6C7960A1903E4",
    "Elector": "",
    "Elected": "",
    "BlockHeight": 1
  }
}
```
###  /txs/tx-block-height/:blockHight  : Get list of block's txs by block height.

```js
/txs/tx-block-height/50
```
```json
{
  "result": [
    {
      "Date": "2022-04-04T10:46:55.412525Z",
      "Hash": "913756634CDB1FEF198557F83FC976D8D54960B539603D3CC6BE9E4B2B5AF299",
      "Elector": "elector048",
      "Elected": "elected048",
      "BlockHeight": 50
    },
    {
      "Date": "2022-04-04T10:46:55.4200758Z",
      "Hash": "EC7F0C3D127EAAD71931471EBED41E3B5895D144E02BDD733A7D5FA2F1689551",
      "Elector": "elector148",
      "Elected": "elected148",
      "BlockHeight": 50
    },
    {
      "Date": "2022-04-04T10:46:55.425978Z",
      "Hash": "9D891D6614616E597926961D510E627B8FC8B757522515C7128505C2305BB6D6",
      "Elector": "elector248",
      "Elected": "elected248",
      "BlockHeight": 50
    },
    {
      "Date": "2022-04-04T10:46:55.4308354Z",
      "Hash": "AAF90C19FEA9FC7D05B058ACA866AF73B982FD9EEC7447B9E024EB6AEAEAF5B8",
      "Elector": "elector348",
      "Elected": "elected348",
      "BlockHeight": 50
    }
  ]
}
```