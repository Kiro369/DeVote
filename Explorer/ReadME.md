# deVote Explorer 
deVote Explorer interfaces with a blockchain node to first extract all the levelDB data. It then stores it in a SQLite database to present the data in a searchable format.

For more details, refer to [Documentation](../Documentation.pdf) - Section 6.9.7 Backend Operations - Page 63:66.

# Installation
Make sure you have [Node.js](https://nodejs.org/en/download) installed.

1. Install dependencies.
```bash
  npm install 
```

2. Set your local variables in .env file.
Make sure to set BLKS_PATH properly, it's where blocks are saved.
```bash
PORT = 9000
SQLITEDB_NAME = "deVote"
BLKS_PATH = "M:\\GradutaionProject\\DeVote\\Blocks"
PROTO_FILE_NAME = "block"
MINS_TO_SYNC_DBS = 0.5
```

3. Start the server, it will be running on your local IP address.
```bash
  npm start 
```

# Using the deVote Explorer API
Query JSON data for blocks, transactions, candidates, status of running election and location of voting machines. 
## /blocks Endpoints

### /blocks/block-height/:blockHeight : Get block metadata by block height.
```js
GET /blocks/block-height/50
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
GET /blocks/block-hash/81D10DABE0F6C51D097DAFA7675473DFC1370621D13C46274706CC457629CFAE
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

## /transactions Endpoints
### /transactions/tx-hash/:txHash : Get single transaction by tx hash.
```js
GET /transactions/tx-hash/66FEF9BBE3B5924923114CC84D46DFE333C3E8F928FF6472108B2FCAC036EE37
```
```json
{
  "result": {
    "Date": 1650144307640,
    "Hash": "66FEF9BBE3B5924923114CC84D46DFE333C3E8F928FF6472108B2FCAC036EE37",
    "Elector": "elector3148",
    "Elected": "elected3148",
    "BlockHeight": 150
  }
}
```
###  /transactions/tx-block-height/:blockHight  : Get list of block's txs by block height.

```js
GET /transactions/tx-block-height/150
```
```json
{
  "result": [
    {
      "Date": 1650144307611,
      "Hash": "672C625CE5F4499088C2D5305BD3752255B882434F73D4FAD1C2D2806E243F34",
      "Elector": "elector0148",
      "Elected": "elected0148",
      "BlockHeight": 150
    },
    {
      "Date": 1650144307622,
      "Hash": "32A493456E2EAF3530A4954D42F97A4E0AFE88814B8F6484B039BBCCC79EE4C7",
      "Elector": "elector1148",
      "Elected": "elected1148",
      "BlockHeight": 150
    },
    {
      "Date": 1650144307629,
      "Hash": "DBAFE2FAA90FC4073D8E8AA3C38DA754D9B4A06B06A687DC4F37AF7CDECE7C99",
      "Elector": "elector2148",
      "Elected": "elected2148",
      "BlockHeight": 150
    },
    {
      "Date": 1650144307640,
      "Hash": "66FEF9BBE3B5924923114CC84D46DFE333C3E8F928FF6472108B2FCAC036EE37",
      "Elector": "elector3148",
      "Elected": "elected3148",
      "BlockHeight": 150
    }
  ]
}
```

</br>

## Get subset of blocks/transactions per request.
Rather than retrieving a complete set of blocks/transactions using a single request which its response will be slow, we are using **bi-directional cursor based pagination technique** that allows the client to ask for one subset at a time and the API responds with the corresponding subset, along with a pagination object for how the clinet can retrieve the next/previous subset of blocks/transactions.

The complete endpoint has the following parameters.
```js
GET /blocks?limit=20&heightCursor=prev_131
GET /transactions?limit=20&timestampCursor=prev_1650144307329
```

- `limit` - The maximum number of blocks to fetch.
 The default limit value is 20.
 The maximum limit value is 50.

- `heightCursor/timestampCursor` - The cursor serving as 
  - a pointer to a specific block/transaction,  where last request left off.
  - an indicator for whether we want set of blocks/transaction next or previous to the pointer.

We are using the unique, sequential columns : **blockHeight** and **transactionTimestamp**.


#### How to use /blocks and /transactions Endpoints:
##### **1- Initial request**
On the initial request, you don't pass a heightCursor/timestampCursor parameter.
You just request the endpoint without parameters.
```js
GET /blocks
GET /transactions
```

Or you can set a limit parameter to specify how many blocks/transactions to retrieve per request.
```js
GET /blocks/?limit=50
GET /transactions/?limit=50
```

**The server's response to the *initial request* will include a pagination object that includes a prev property when there are additional blocks/transactions to be retrieved.**

**You use prev's link to request more previous blocks/transactions.**

```json
 "pagination": {
    "prev": "https://devote-explorer-backend.herokuapp.com/blocks?limit=20&heightCursor=prev_131",
    "more": true,
    "max": 150
  }

  "pagination": {
    "prev": "https://devote-explorer-backend.herokuapp.com/transactions?limit=20&timestampCursor=prev_1650144307329",
    "more": true,
    "max": 1650144307640
  }
```

<details>
  <summary>Show full response for /blocks</summary>

```json
{
  "pagination": {
    "prev": "https://devote-explorer-backend.herokuapp.com/blocks?limit=20&heightCursor=prev_131",
    "more": true,
    "max": 150
  },
  "result": [
    {
      "Height": 150,
      "PrevHash": "6ED794D084F6B99B5088798F0E62FA5AA537D673665B79FFC018B08A95446535",
      "Timestamp": 1650144307652,
      "MerkleRoot": "6BEFEC9C11B0B02C27BC1E68A1CDD2C7919832FC8AC3B887E140D3AE51856534",
      "Hash": "43034489A9AEDE4150562E244C3D4CC526B8E7597E444A5D78965ECF653EB020",
      "Miner": "Test33",
      "nTx": 4
    },...
    {
      "Height": 131,
      "PrevHash": "9321074BEE883F85B7977A750857E569E4E497E336B7029CDBFE37754FD32101",
      "Timestamp": 1650144306527,
      "MerkleRoot": "9D66249A7A1D2778744C8DDF695C416F2C40FD294EFCF12C12544E363C436ED9",
      "Hash": "F2AAA55AD5A6C9C002DB70E0BBCC4C3602E5A86E481697B8B22F60BDB0AFA334",
      "Miner": "Test33",
      "nTx": 4
    }
  ]
}
```
</details>


<details>
  <summary>Show full response for /transactions </summary>

```json
{
  "pagination": {
    "prev": "https://devote-explorer-backend.herokuapp.com/transactions?limit=20&timestampCursor=prev_1650144307329",
    "more": true,
    "max": 1650144307640
  },
  "result": [
    {
      "Date": 1650144307640,
      "Hash": "66FEF9BBE3B5924923114CC84D46DFE333C3E8F928FF6472108B2FCAC036EE37",
      "Elector": "elector3148",
      "Elected": "elected3148",
      "BlockHeight": 150
    },...
    {
      "Date": 1650144307329,
      "Hash": "00DE2585B9FA5208A020F942B548327AC03C4B2A8B00F5C50F39D712BF0DADF7",
      "Elector": "elector0144",
      "Elected": "elected0144",
      "BlockHeight": 146
    }
  ]
}
```
</details>

<br>

##### **2- Subsequent requests  :**
On subsequent requests, there should be a heightCursor/timestampCursor parameter
```js
GET /blocks?limit=20&heightCursor=prev_131
GET /transactions?limit=20&timestampCursor=prev_1650144307329
```
**The server's response to the *subsequent request* will include a pagination object that includes prev and next properties when there are additional blocks/transactions to be retrieved.**

**Response :**
```json

  "pagination": {
    "prev": "https://devote-explorer-backend.herokuapp.com/blocks?limit=20&heightCursor=prev_111",
    "next": "https://devote-explorer-backend.herokuapp.com/blocks?limit=20&heightCursor=next_130",
    "more": true,
    "max": 150
  }

 "pagination": {
    "prev": "https://devote-explorer-backend.herokuapp.com/transactions?limit=20&timestampCursor=prev_1650144307018",
    "next": "https://devote-explorer-backend.herokuapp.com/transactions?limit=20&timestampCursor=next_1650144307287",
    "more": true,
    "max": 1650144307640
  }
```
**Note that the pagination's object includes a ***next*** property, which points to block height of the next page.**

**On your next request, you can either fetch prev or next set of blocks/transactions**

**For example, to retrieve the next ***older*** set of the blocks [Block 110 : Block 91] .** 
- Request the link of the *prev* property you received on the last request.
```js
GET /blocks?limit=20&heightCursor=prev_111
```

**to retrieve the next ***newer*** set of the blocks [Block 150 : Block 131].**
- Request the link of the *next* property you received on the last request.
```js
GET /blocks?limit=20&heightCursor=next_130
```
<br>

##### **3- Final request  :**

**As you make more subsequent requests to retrieve previous blocks/transactions, you will eventually receive a response with ```more``` as ```false```, indicating the end of the entire set.**

```json
{
  "pagination": {
    "next": "https://devote-explorer-backend.herokuapp.com/blocks?limit=20&heightCursor=next_10",
    "more": false,
    "max": 150
  },
  "result": [
    {
      "Height": 10,
      "PrevHash": "25C651CA1EA3C903E01D7F945F833202495690358EB1882EA15FC491EA322AB1",
      "Timestamp": 1650144300173,
      "MerkleRoot": "5A3EFD3316319EB0953EE5DAA4B7848CCAC6AFF35EF2B4642FD7BC9837E6D77E",
      "Hash": "9F9982478BF2AF82085061D58ECFB7EA4DD12B40CB9B1BC5E524BBE9D1A5F0E7",
      "Miner": "Test33",
      "nTx": 4
    },...
    {
      "Height": 1,
      "PrevHash": null,
      "Timestamp": 1650144298861,
      "MerkleRoot": null,
      "Hash": "B60D6D9A62B48D4B48D3F6500B6552F3A6C2B772949E94C5C3BAB0F6CF9BE77B",
      "Miner": "deVote",
      "nTx": 1
    }
  ]
}
```

## Errors of /blocks /transactions endpoints 
If there are any errors, the server's response will includes an errors object having a list of errors.

```js
/blocks/block-height/200
```
```json
{
  "errors": [
    {
      "code": "404",
      "status": "Not Found",
      "detail": "No block was found for height : 200"
    }
  ]
}
```
```js
/blocks/block-hash/RT0D6D9A62B48D4B48D3F6500B6552F3A6C2B772949E94C5C3BAB0F6CF9BE77B
```
```json
{
  "errors": [
    {
      "code": "404",
      "status": "Not Found",
      "detail": "No block was found for hash : RT0D6D9A62B48D4B48D3F6500B6552F3A6C2B772949E94C5C3BAB0F6CF9BE77B"
    }
  ]
}
```

```js
/transactions/tx-block-height/200
```
```json
{
  "errors": [
    {
      "code": "404",
      "status": "Not Found",
      "detail": "No transaction was found for blockHeight : 200"
    }
  ]
}
```
```js
/transactions/tx-hash/RT0D6D9A62B48D4B48D3F6500B6552F3A6C2B772949E94C5C3BAB0F6CF9BE77B
```
```json
{
  "errors": [
    {
      "code": "404",
      "status": "Not Found",
      "detail": "No transaction was found for hash : RT0D6D9A62B48D4B48D3F6500B6552F3A6C2B772949E94C5C3BAB0F6CF9BE77B"
    }
  ]
}
```

## /vms Endpoints

### POST /vms  : Add a new voting machine.
**The required properties of json body are**
  - id 
  - name
  - lat
  - lng

```js
POST /vms
request body : (application/json)
{
  "id": "0F0F1C30FBB75B87EB11A4934A9520FEF31D43609E3A1261FF63A31D73347F4F1828",
  "name": "Machine1",
  "lat": "39.354503136666662",
  "lng": "33.313432350000007"
}
```

```json
{
    "result": {
        "id": "0F0F1C30FBB75B87EB11A4934A9520FEF31D43609E3A1261FF63A31D73347F4F1828",
        "lat": "39.354503136666662",
        "lng": "33.313432350000007",
        "name": "Machine1"
    }
}
```

### GET /vms  : Get list of voting machines.

```js
GET /vms
```

```json
{
  "result": [
 {
        "id": "0F0F1C30FBB75B87EB11A4934A9520FEF31D43609E3A1261FF63A31D73347F4F1828",
        "lat": "39.354503136666662",
        "lng": "33.313432350000007",
        "name": "Machine1"
    }
  ]
}
```

### Errors of adding invalid voting machine.
```js
POST /vms
request body : (application/json)
{
    "id": "500",
    "name": "VM1",
    "lat": "40.286548",
    "lng": "44.739853"
}
```

```json
{
  "request": {
    "id": "500",
    "name": "VM1",
    "lat": "40.286548",
    "lng": "44.739853"
  },
  "errors": [
    {
      "code": "400",
      "status": "Bad Request",
      "detail": "Your request parameters values are invalid"
    }
  ]
}
```