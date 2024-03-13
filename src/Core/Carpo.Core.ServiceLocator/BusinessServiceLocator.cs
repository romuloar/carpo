using Carpo.Core.Interface.Context;
using Carpo.Core.Interface.Domain;
using Carpo.Core.Interface.ServiceLocator;
using Carpo.Core.ServiceLocator.Base;

namespace Carpo.Core.ServiceLocator
{
    /// <summary>
    /// UnitOfWorkService
    /// </summary>
    public class BusinessServiceLocator : BaseServiceLocator<IServiceDomain>, IDisposable, IBusinessServiceLocator
    {

        #region Attributes and Properties    

        /// <summary>
        /// _uowRepository
        /// </summary>
        private IRepositoryServiceLocator _rep { get; set; }
        /// <summary>
        /// UowRepository
        /// </summary>
        public IRepositoryServiceLocator Rep
        {
            get
            {
                //todo -> Remove dependency of Carpo.Core.EFCore
                _rep = _rep ?? new UnitOfWorkRepositoryServiceLocator(this.Context);
                return _rep;
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public BusinessServiceLocator(ICarpoContext context) : base(context) { }

        /// <summary>
        /// 
        /// </summary>        
        public TEntity Get<TEntity>() where TEntity : IServiceDomain
        {
            return GetSer<TEntity>(this);
        }

        /// <summary>
        /// UnitOfWorkService
        /// </summary>
        ~BusinessServiceLocator()
        {
            // Dispose
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
