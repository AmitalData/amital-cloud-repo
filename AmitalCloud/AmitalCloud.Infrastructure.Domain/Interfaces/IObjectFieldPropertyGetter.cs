namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IObjectFieldPropertyGetter
    {
        string GetObjectFieldType(string fieldName, string objectTableName, int tenant);
    }

}
