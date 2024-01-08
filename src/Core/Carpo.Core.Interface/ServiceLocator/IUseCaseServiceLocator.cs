using Carpo.Core.Interface.UseCase;

namespace Carpo.Core.Interface.ServiceLocator
{
    public interface IUseCaseServiceLocator
    {
        public TEntity Get<TEntity>(params IContractUseCase[] list) where TEntity : IUseCaseDomain;
    }
}
