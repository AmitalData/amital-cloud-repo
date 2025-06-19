using Unity;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public sealed class InjectionContainer
    {
        private static readonly InjectionContainer instance = new InjectionContainer();
        private static readonly IUnityContainer _container = new UnityContainer();
        private InjectionContainer()
        {
        }
        public InjectionContainer Instance { get { return instance; } }
        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}
