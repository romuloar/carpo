using Carpo.Core.Interface.Context;
using Carpo.Core.Interface.Domain;
using Carpo.Core.Interface.ServiceLocator;
using Carpo.Core.ServiceLocator.Base;

namespace Carpo.Core.ServiceLocator
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class UnitOfWorkRepositoryServiceLocator :  BaseServiceLocator<IRepositoryDomain>, IRepositoryServiceLocator, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>        
        public UnitOfWorkRepositoryServiceLocator(ICarpoContext context) : base(context)
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>        
        public TEntity Get<TEntity>() where TEntity : IRepositoryDomain
        {
            return GetSer<TEntity>(this.Context, this);
        }
      
        /// <summary>
        /// Destructor
        /// </summary>
        ~UnitOfWorkRepositoryServiceLocator()
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
