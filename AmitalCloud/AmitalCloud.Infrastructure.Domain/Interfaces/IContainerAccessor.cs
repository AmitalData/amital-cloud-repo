
namespace AmitalCloud.Infrastructure.Domain.Interfaces
{

    public interface IContainerAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
}
