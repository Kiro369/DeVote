const { open } = require('sqlite')
const sqlite3 = require('sqlite3')
const path = require('path');
const fs = require('fs');
const { _success, _warn, _info, _breaker, _time } = require("../../misc/logger")

class SQLite {

    async openSQLiteDB(dbName) {
        this.db = await open({
            filename: path.join(__dirname, `${dbName}.sqlite`),
            driver: sqlite3.Database
        }).catch(err => { throw err; })

        await this.db.migrate({
            migrationsPath: path.join(__dirname, 'migrations'),
        }).catch(err => { throw err; })

        _success(`Connection to ${dbName} SQLiteDB is successful.`)
        return this
    };

    async closeSQLiteDB() {
        this.db.close()
            .then(() => console.log('Connection to SQLiteDB is closed.'))
            .catch(err => { throw err; })
    };

    async getLatestBlockHeight() {
        const lbh = await this.db.get("SELECT MAX(Height) FROM Blocks;")
        return lbh["MAX(Height)"] || 0
    };

    async getOldestBlockHeight() {
        const lbh = await this.db.get("SELECT MIN(Height) FROM Blocks;")
        return lbh["MIN(Height)"] || 0
    };

    async getBlocksByHeightCursorAndLimit(heightCursor, limit, operator, order) {
        const SQL_QUERY = `
        SELECT * FROM Blocks
        WHERE Height ${operator} ?
		ORDER BY Height ${order}
        LIMIT ?`
        const blocks = await this.db.all(SQL_QUERY, heightCursor, limit);
        return blocks
    }

    async insertBlock(block) {
        let { Height, PrevHash, Timestamp, MerkleRoot, Hash, Miner, nTx } = block;
        if (nTx == null) nTx = 0;
        if (Miner == null) Miner = "";
        _info(`\nInserting Block : Hash ${Hash} -  Height ${Height}`)
        await this.db.run(
            "INSERT INTO Blocks VALUES (?,?,?,?,?,?,?)",
            Height, PrevHash, Timestamp, MerkleRoot, Hash, Miner, nTx
        );
        _success(`Done inserting Block ${Height}`)

        let { Transactions } = block;
        if (Transactions == null) {
            // console.log("transactions", Transactions)
            _warn(`No Transactions Found in Block ${Height}`)
            return
        }
        _info(`Block ${Height} has ${Transactions.length} Transactions`)
        await this.insertTxs(Transactions, Height)
        _success(`Done inserting ${Transactions.length} Transactions`)
    };

    async insertBlocks(blocks) {
        for (let index = 0; index < blocks.length; index++) {
            const block = blocks[index];
            await this.insertBlock(block)
        }
    };

    async getBlockByHeight(height) {
        let targetBlock = await this.db.get("SELECT * FROM Blocks WHERE Height == ?", height)
        return targetBlock
    };

    async getBlockByHash(hash) {
        let targetBlock = await this.db.get("SELECT * FROM Blocks WHERE Hash == ?", hash)
        return targetBlock
    };

    async getLatestTxTimestamp() {
        const ltt = await this.db.get("SELECT MAX(Date) FROM Transactions")
        return ltt["MAX(Date)"] || 0
    };

    async getOldestTxTimestamp() {
        const ltt = await this.db.get("SELECT MIN(Date) FROM Transactions")
        return ltt["MIN(Date)"] || 0
    };

    async getTxsByTimestampCursorAndLimit(timestampCursor, limit, operator, order) {
        const SQL_QUERY = `
        SELECT * FROM Transactions
        WHERE Date ${operator} ?
		ORDER BY Date ${order}
        LIMIT ?`
        const txs = await this.db.all(SQL_QUERY, timestampCursor, limit);
        return txs
    }

    async insertTx(tx, blockHeight) {
        let { Hash, Date, Elector, Elected, Confirmations } = tx;
        if (!Confirmations) Confirmations = 0;
        _info(`Inserting Transaction : Hash ${Hash} -  blockHeight ${blockHeight}`)
        this.db.run('PRAGMA foreign_keys = OFF;');
        await this.db.run("INSERT INTO Transactions VALUES (?,?,?,?,?,?)",
            Date, Hash, Elector, Elected, Confirmations, parseInt(blockHeight)
        )
        this.db.run('PRAGMA foreign_keys = ON;');
    };

    async insertTxs(txs, blockHeight) {
        for (let index = 0; index < txs.length; index++) {
            const tx = txs[index];
            await this.insertTx(tx, blockHeight)
        }
    };

    async getTxByHash(hash) {
        let targetTx = await this.db.get("SELECT * FROM Transactions WHERE Hash == ?", hash)
        return targetTx
    };

    async getTxsByBlockHeight(blockHeight) {
        let targetTxs = await this.db.all("SELECT * FROM Transactions WHERE BlockHeight == ?", blockHeight)
        return targetTxs
    };

    async insertVM({ id, lat, lng, address, governorate }) {
        // check if vm's governorate exists in election's governorate list
        const GovernorateExists = await this.DoesGovernorateExist(governorate);
        console.log(GovernorateExists)
        if (!GovernorateExists) {
            let err = new Error('Governorate does not exist');
            err.errno = 10011;
            err.message = `No Governorate matches ${governorate}`;
            throw err;
        }

        await this.db.run(
            "INSERT INTO VMachines  VALUES (?,?,?,?,?)",
            id, lat, lng, address, governorate
        )
        // also add id of vm to certain governorate
        await this.addIDofVMToGovernorate(governorate, id)
    };

    async getVMs() {
        let vms = await this.db.all("SELECT * FROM VMachines")
        // if (!vms.length) throw new Error("No VMs Found")
        return vms
    }

    async insertCandidate({ id, name, color }) {
        await this.db.run(
            "INSERT INTO Candidates (ID,Name,NoVotes,Color) VALUES (?,?,?,?)",
            id, name, 0, parseInt(color)
        )
    }

    async getCandidates() {
        return await this.db.all("Select * From Candidates")
    }

    async setNoVotesForCandidates() {
        // get list of candidates 
        const candidates = await this.db.all("SELECT * FROM Candidates")
        if (!candidates.length) {
            _warn("There are no candidates added at the moment.")
            return
        }
        // for each candidate, get votes count from transactions table.
        for (let index = 0; index < candidates.length; index++) {
            const { ID, NoVotes } = candidates[index];
            const noVotesObj = await this.db.get("SELECT COUNT(Elected) FROM Transactions Where Elected == ?", ID)
            const NewNoVotes = noVotesObj['COUNT(Elected)'];
            await this.UpdateNoVotesForCandidate(ID, NoVotes, NewNoVotes)
        }
    }

    async UpdateNoVotesForCandidate(id, oldNoVotes, newNoVotes) {
        const SQL_QUERY = `UPDATE Candidates SET NoVotes = ?  WHERE ID == "${id}"`;
        await this.db.run(SQL_QUERY, newNoVotes)
        console.log(`Candidate: ${id}, Old noVotes: ${oldNoVotes} New noVotes: ${newNoVotes}`)
    }

    async insertInitialGovernorates() {
        let currentgovernorateList = await this.getGovernorates();
        if (currentgovernorateList.length) {
            _time("Governorates is already inserted and set.")
            return;
        }

        const governorateJson = fs.readFileSync(path.join(__dirname, "../../misc/governorates.json"))
        const governorateList = JSON.parse(governorateJson)
        // console.log("governorateList", governorateList)
        for (let index = 0; index < governorateList.length; index++) {
            const { ar, en } = governorateList[index];
            await this.insertGovernorate({ ar, en })
        }
        _time("Governorates has been inserted and set.")
    }

    async insertGovernorate({ ar, en }) {
        await this.db.run("INSERT INTO Governorates (ArabicName,EnglishName,Votes,Color) VALUES (?,?,?,?)",
            ar, en, "[]", global.defaultTieColor)
    }

    async getGovernorates() {
        return await this.db.all("Select * From Governorates")
    }

    async getGovernorates_API() {
        const governorates = await this.db.all("Select ArabicName,EnglishName,Color,Votes From Governorates");
        const governorateList = governorates.map((governorate, index) => {
            const { Votes } = governorate;
            governorate["Votes"] = JSON.parse(Votes)
            return governorate
        })
        return governorateList
    }

    async DoesGovernorateExist(governorate) {
        const DoesGovernorateExist = await this.db.get("SELECT EnglishName FROM Governorates Where EnglishName == ?", governorate)
        if (!DoesGovernorateExist) return false
        else return true
    }

    async getIDsofVMsForGovernorate(governorate) {
        const { IDsOfVMs } = await this.db.get("SELECT IDsOfVMs FROM Governorates Where EnglishName == ?", governorate)
        return IDsOfVMs
    }

    // invoked every time a vm got added
    async addIDofVMToGovernorate(governorate, newID) {
        // first get governorates's current ids of vms 
        const currentIDs = await this.getIDsofVMsForGovernorate(governorate);
        // append new vm id then update
        let newIDs = "";
        // if current ids is empty
        if (!currentIDs) newIDs = newID;
        else newIDs = currentIDs + "," + newID;
        await this.UpdateIDsofVMsForGovernorate(governorate, currentIDs, newIDs);
    }

    async UpdateIDsofVMsForGovernorate(governorate, oldIDs, newIDs) {
        const SQL_QUERY = `UPDATE Governorates SET IDsOfVMs = ?  WHERE EnglishName == "${governorate}"`;
        await this.db.run(SQL_QUERY, newIDs)
        // console.log(`Governorate:  ${governorate} | Old IDs: ${oldIDs} | New newIDs: ${newIDs}`)
    }

    async UpdateVotesForGovernorate(governorate, oldVotes, newVotes) {
        const SQL_QUERY = `UPDATE Governorates SET Votes = ?  WHERE EnglishName == "${governorate}"`;
        await this.db.run(SQL_QUERY, newVotes)
        _success(`Updated  Votes for Governorate : ${governorate}`)
        // console.log(`Governorate: ${governorate} | Old noVotes: ${oldVotes} | New newIDs: ${newVotes}`)
    }

    async UpdateColorForGovernorate(governorate, oldColor, newColor) {
        const SQL_QUERY = `UPDATE Governorates SET Color = ?  WHERE EnglishName == "${governorate}"`;
        await this.db.run(SQL_QUERY, newColor)
        _success(`Updated  Color for Governorate : ${governorate}`)
        // console.log(`Governorate: ${governorate} | Old Color: ${oldColor} | New Color: ${newColor}`)
    }

    async setNoVotesAndColorForNullGovernorates() {
        // get list on null ids governorates
        const nullGovernorateList = await this.db.all("SELECT EnglishName,Color,Votes FROM Governorates Where IDsOfVMs IS NULL ")
        _info(`Setting Votes,Color for ${nullGovernorateList.length} governorates having no voting machine`)
        for (let index = 0; index < nullGovernorateList.length; index++) {
            const { EnglishName, Color, Votes } = nullGovernorateList[index];
            let candidateList = []
            const candidates = await this.getCandidates();
            for (let index = 0; index < candidates.length; index++) {
                const { ID, Name } = candidates[index];
                candidateList.push({ "ID": ID, "Name": Name, "NoVotes": 0 })
            }
            await this.UpdateVotesForGovernorate(EnglishName, Votes, JSON.stringify(candidateList))
            await this.UpdateColorForGovernorate(EnglishName, Color, global.defaultTieColor)
        }
        _success(`Done Setting Votes,Color`)
    }

    async setNoVotesForGovernorates() {
        // first set 0 votes and default color for null-ids governorates.
        await this.setNoVotesAndColorForNullGovernorates();

        // get the non null-ids governorates.
        const governorateList = await this.db.all("SELECT EnglishName,IDsOfVMs,Votes,Color FROM Governorates Where IDsOfVMs IS NOT NULL ")
        if (!governorateList.length) {
            _warn("There are no voting machines added at the moment to any governorates.")
            return
        }

        const candidates = await this.getCandidates();
        if (!candidates.length) {
            _warn("There are no candidates added at the moment.")
            return
        }

        for (let index = 0; index < governorateList.length; index++) {
            _breaker();

            const { IDsOfVMs, EnglishName, Votes, Color } = governorateList[index];
            // get list of vm ids of each governorate.
            const vmIDList = IDsOfVMs.split(",");
            console.log(`Governorate: ${EnglishName} has ${vmIDList.length} voting machines`)
            console.log(`vmIDList: ${vmIDList}`)

            // a list of candidates object {ID,Name,NoVotes} for each governorate.
            let candidateList = [];
            let maxNoVotes = 0;
            let dominantColor = global.defaultTieColor;

            for (let candidateIndex = 0; candidateIndex < candidates.length; candidateIndex++) {
                let TotalNoVotesForCandidate = 0;

                let candidate = candidates[candidateIndex];
                const candidateID = candidate["ID"];
                const candidateName = candidate["Name"]
                const candidateColor = candidate["Color"]

                // for each candidate, get number of votes from each voting machine.
                for (let index = 0; index < vmIDList.length; index++) {
                    const vmID = vmIDList[index];
                    const SQL_QUERY = `SELECT COUNT(Elector) FROM Transactions Where Elector == "${vmID}" AND Elected == "${candidateID}"`;
                    const noVotesObj = await this.db.get(SQL_QUERY)
                    const noVotesByVM = noVotesObj['COUNT(Elector)'];
                    TotalNoVotesForCandidate += parseInt(noVotesByVM);
                    console.log(`candidate ${candidateID} has ${noVotesByVM} votes from VM ${vmID}`)
                }

                // set the dominant color for each governorate.
                if (TotalNoVotesForCandidate > maxNoVotes) {
                    maxNoVotes = TotalNoVotesForCandidate;
                    dominantColor = candidateColor
                }
                candidateList.push({ "ID": candidateID, "Name": candidateName, "NoVotes": TotalNoVotesForCandidate });
            }

            console.log("candidateList", candidateList)

            if (dominantColor == global.defaultTieColor) {
                _info(`It is a tie`)
            }

            // update dominant candidate color and votes for each governorate.
            _info(`Current  Color  : ${Color}`)
            _info(`Dominant Color  : ${dominantColor}`)

            _info(`Updating Color for Governorate : ${EnglishName}`)
            await this.UpdateColorForGovernorate(EnglishName, Color, dominantColor);

            _info(`Updating Votes for Governorate : ${EnglishName}`)
            await this.UpdateVotesForGovernorate(EnglishName, Votes, JSON.stringify(candidateList))
        }
    }
}

module.exports = { SQLite }