using LevelDB;
using ProtoBuf;

namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    class TransactionsDLT
    {
        public static TransactionsDLT Current = new TransactionsDLT();
        private static DB LevelDB  = new DB(new Options { CreateIfMissing = true }, Settings.Current.TransactionsDLTPath);

        public static void AddTxRecord(TransactionRecord transactionRecord)
        {
            LevelDB.Put(transactionRecord.Hash, TransactionRecord.Serialize(transactionRecord));
        }

        public static byte[] getTxRecord(byte[] hash)
        {
          return LevelDB.Get(hash);
        }
    }
}
