using Carpo.Core.Interface.UseCase;

namespace Carpo.Core.Interface.ServiceLocator
{
    public interface IAppBusinessServiceLocator
    {
        public IUseCaseServiceLocator UowUseCase { get; }
        public TEntity Get<TEntity>(IRepositoryUseCase contract) where TEntity : IApplicationUseCase;
    }
}
