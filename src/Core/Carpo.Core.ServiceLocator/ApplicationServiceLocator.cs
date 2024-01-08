using Carpo.Core.Interface.Context;
using Carpo.Core.Interface.Domain;
using Carpo.Core.Interface.ServiceLocator;
using Carpo.Core.ServiceLocator.Base;

namespace Carpo.Core.UnitOfWork
{
    /// <summary>
    /// Application class - UnitOfWorkApplication
    /// </summary>
    public class ApplicationServiceLocator : BaseServiceLocator<IApplicationDomain>, IDisposable, IApplicationServiceLocator
    {
        #region Propriedades        

        /// <summary>
        /// BusinessServiceLocator
        /// </summary>
        private IBusinessServiceLocator _bus { get; set; }

        /// <summary>
        /// BusinessServiceLocator
        /// </summary>
        public IBusinessServiceLocator Bus
        {
            get
            {
                _bus = _bus ?? new BusinessServiceLocator(this.Context);
                return _bus;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ApplicationServiceLocator(ICarpoContext context): base(context)
        {
                
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>        
        public TEntity Get<TEntity>() where TEntity : IApplicationDomain
        {
            return GetSer<TEntity>(this);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ApplicationServiceLocator()
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
