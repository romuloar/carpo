using Carpo.Core.Interface.Context;
using Carpo.Core.Interface.Domain;
using Carpo.Core.Interface.ServiceLocator;
using Carpo.Core.Interface.UseCase;
using Carpo.Core.ServiceLocator.Base;

namespace Carpo.Core.ServiceLocator
{
    /// <summary>
    ///  UnitOfWorkUseCase
    /// </summary>
    public class UseCaseServiceLocator : BaseServiceLocator<IUseCaseDomain>, IDisposable, IUseCaseServiceLocator
    {
        #region Constructor

        /// <summary>
        /// Contructor 
        /// </summary>
        public UseCaseServiceLocator(ICarpoContext context) : base(context)
        {

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>        
        public TEntity Get<TEntity>(params IContractUseCase[] list) where TEntity : IUseCaseDomain
        {
            return this.GetSer<TEntity>(this, list);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~UseCaseServiceLocator()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
