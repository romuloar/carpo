using Carpo.Core.Interface.Domain;

namespace Carpo.Core.Interface.ServiceLocator
{
    public interface IBusinessServiceLocator
    {
        public IRepositoryServiceLocator Rep { get; }
        public TEntity Get<TEntity>() where TEntity : IServiceDomain;
    }
}
