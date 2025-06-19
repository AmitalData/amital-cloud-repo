namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class UpdateEntityArgs
    {
        public object EntityPM { get; set; }
        public string EntityName { get; set; }
        public int Tenant { get; set; }
        public string LoggedUserEmail { get; set; }
    }

}
