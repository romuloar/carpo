namespace Carpo.Core.Interface.ServiceLocator
{
    public interface IBaseServiceLocator<TEntityMap>
    {
        protected TEntity GetSer<TEntity>(params object[] list) where TEntity : TEntityMap; 
        
    }
}
