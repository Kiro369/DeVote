namespace DeVote
{
    public class Transaction
    {
        public string Elector { get; set; }
        public string Elected { get; set; }

        public Transaction(string elector, string elected)
        {
            this.Elector = elector;
            this.Elected = elected;
        }
    }
}
