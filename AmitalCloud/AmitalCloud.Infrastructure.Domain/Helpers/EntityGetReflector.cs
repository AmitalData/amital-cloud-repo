namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class EntityGetReflector
    {
        public string EntityName { get; set; }
        public object[] Parameters { get; set; }
        public string MethodName { get; set; }
        public int Tenant { get; set; }
    }
}
