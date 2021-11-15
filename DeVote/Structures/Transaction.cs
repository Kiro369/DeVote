using System;

namespace DeVote.Structures
{
    public class Transaction
    {
        public int ID { get; set; }
        public string Date;
        public string Elector { get; set; }
        public string Elected { get; set; }

        // public DateTime timeStamp;

        public Transaction(int id, string date, string elector, string elected)
        {
            this.ID = id;
            this.Date = date;
            this.Elector = elector;
            this.Elected = elected;
        }
    }
}