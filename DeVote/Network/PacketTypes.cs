namespace DeVote.Network
{
    public enum PacketTypes : short
    {
        None = 0,
        Test = 1,
        Transaction = 1000,
        Confirmation = 1001,
        LRNConsensus = 1002,
        AddBlock = 1003,
        LatestHeight = 1003,
        GetBlock = 1004,
        TransactionData = 1005
    }
}
