using LevelDB;
using ProtoBuf;
using System.Text;

namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    class TransactionsDLT
    {
        public static TransactionsDLT Current = new TransactionsDLT();
        public DB LevelDB;

        private TransactionsDLT()
        {
            LevelDB = new DB(new Options { CreateIfMissing = true }, Settings.Current.TransactionsDLTPath);
        }

        public void AddRecord(TransactionRecord transactionRecord)
        {
            LevelDB.Put(transactionRecord.Hash, TransactionRecord.Serialize(transactionRecord));
        }
        public TransactionRecord GetRecord(byte[] hash)
        {
            return TransactionRecord.Deserialize(LevelDB.Get(hash));
        }
        public TransactionRecord GetRecord(string hash)
        {
            return GetRecord(Encoding.UTF8.GetBytes(hash));
        }
    }
}
