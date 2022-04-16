namespace DeVote.Network
{
    public enum PacketTypes : short
    {
        None = 0,
        Test = 1,
        Transaction = 1000,
        Confirmation = 1001,
        LRNConsensus = 1002,
        Mine = 1003
    }
}
