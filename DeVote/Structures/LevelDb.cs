using LevelDB;

namespace DeVote.Structures
{
    public class LevelDb
    {
        public DB db;

        public LevelDb(string dbPath)
        {
            // Open a connection to a new DB and create if not found
            var options = new Options { CreateIfMissing = true };
            this.db = new DB(options, dbPath);
        }
    }
}