using Carpo.Core.Interface.Context;
using Carpo.Core.Interface.Domain;
using Carpo.Core.Interface.ServiceLocator;
using System.Reflection;

namespace Carpo.Core.ServiceLocator.Base
{
    public class BaseServiceLocator<TEntityMap> : IBaseServiceLocator<TEntityMap>
    {
        protected ICarpoContext Context { get; private set; }

        public BaseServiceLocator(ICarpoContext context)
        {
            this.Context = context;
        }
        public TEntity GetSer<TEntity>(params object[] list) where TEntity : TEntityMap
        {
            List<object> parameters = new List<object>();
            parameters.Add(list[0]);

            var param2 = (object[])list[1];

            foreach (var item in param2)
            {
                parameters.Add(item);
            }

            var map = Activator.CreateInstance(typeof(TEntity), parameters.ToArray());
            return (TEntity)map;
        }

    }
}
