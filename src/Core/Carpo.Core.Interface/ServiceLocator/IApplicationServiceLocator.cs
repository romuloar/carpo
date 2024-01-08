using Carpo.Core.Interface.Domain;

namespace Carpo.Core.Interface.ServiceLocator
{
    public interface IApplicationServiceLocator
    {
        public IBusinessServiceLocator Bus { get; }
        public TEntity Get<TEntity>() where TEntity : IApplicationDomain;
    }
}
