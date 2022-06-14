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
            LevelDB = new DB(new Options { CreateIfMissing = true }, Settings.Current.VotedDLTPath);
        }
        public void Add(string ID, string MachineID = "")
        {
            if (string.IsNullOrEmpty(MachineID))
                MachineID = Constants.MachineID;
            LevelDB.Put(Argon2.ComputeHash(ID), MachineID);
        }
        public bool Contains(string hashedID)
        {
            var machineVotedOn = LevelDB.Get(hashedID);
            return !string.IsNullOrEmpty(machineVotedOn);
        }
    }
}
