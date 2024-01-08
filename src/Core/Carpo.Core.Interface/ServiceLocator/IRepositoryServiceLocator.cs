using Carpo.Core.Interface.Domain;

namespace Carpo.Core.Interface.ServiceLocator
{
    public interface IRepositoryServiceLocator
    {
        public TEntity Get<TEntity>() where TEntity : IRepositoryDomain;
    }
}
