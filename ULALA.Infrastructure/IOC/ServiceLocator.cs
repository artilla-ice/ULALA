using System;
using Unity;
                
namespace ULALA.Infrastructure.IOC
{
    public class ServiceLocator
    {
        private static ServiceLocator m_currentInstance;
        public static ServiceLocator Current
        {
            get
            {
                if (m_currentInstance == null)
                    m_currentInstance = new ServiceLocator();
                return m_currentInstance;
            }
        }

        public T Resolve<T>()
        {
            return this.Container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return this.Container.Resolve(type);
        }

        public static void SetContainer(IUnityContainer container)
        {
            Current.Container = container;
        }

        private IUnityContainer Container { get; set; }
    }
}
