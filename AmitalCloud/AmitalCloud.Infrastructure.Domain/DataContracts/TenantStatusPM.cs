public class TenantStatusPM
{
    public bool DoBlocking { get; set; }
    public BlockingType BlockType { get; set; } = BlockingType.None;
    public int? SuspendDaysLeft { get; set; }
    public int? TrialDaysLeft { get; set; }
    public int? PaidDaysLeft { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? ExpirationDaysLeft { get; set; }



}

public enum BlockingType
{
    None,
    Suspend,
    Company,
    User
}
