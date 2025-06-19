namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IObjectTablePropertyGetter
    {
        string GetKeyPropertyPath(string objectTableName, int tenant);
    }
}
