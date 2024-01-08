using Carpo.Core.Interface.Context;
using Carpo.Core.Interface.ServiceLocator;
using Carpo.Core.ServiceLocator.Base;

namespace Carpo.Core.ServiceLocator
{
    /// <summary>
    /// GenericServiceLocator
    /// </summary>
    public class GenericServiceLocator : BaseServiceLocator<IGenericServiceLocator>, IDisposable
    {
        public GenericServiceLocator(ICarpoContext context):base(context) { }
        /// <summary>
        /// 
        /// </summary>        
        public TEntity Get<TEntity>() where TEntity : IGenericServiceLocator
        {
            return GetSer<TEntity>(this);
        }

        /// <summary>
        /// GenericServiceLocator
        /// </summary>
        ~GenericServiceLocator()
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
