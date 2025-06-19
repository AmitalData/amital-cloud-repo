namespace AmitalCloud.Infrastructure.Domain.Enums
{
    public enum SignMethodByQueueEnum
    {
        None = 0,
        MemorySignQueue,
        HybridDbSignQueue,
        HSMSignQueue,
    }

}
