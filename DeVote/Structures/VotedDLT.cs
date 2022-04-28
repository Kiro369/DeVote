using DeVote.Cryptography;
using LevelDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Structures
{
    public class VotedDLT
    {
        public static VotedDLT Current = new();
        public DB LevelDB;
        private VotedDLT()
        {
            LevelDB = new DB(new Options { CreateIfMissing = true }, Constants.VotedDLTPath);
        }
        public void Add(string ID, string MachineID = "")
        {
            if (MachineID == "")
                MachineID = Constants.MachineID;
            LevelDB.Put(Argon2.ComputeHash(ID), MachineID);
        }
        public bool Contains(string ID)
        {
            var hashedID = Argon2.ComputeHash(ID);
            var machineVotedOn = LevelDB.Get(hashedID);
            return !string.IsNullOrEmpty(machineVotedOn);
        }
    }
}
