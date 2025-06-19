namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IDomainServiceUpdateClass<TEntityPM>
    {
        void Insert(TEntityPM entityPM);
        void Update(TEntityPM entityPM);
        void Delete(TEntityPM entityPM);

    }
}
