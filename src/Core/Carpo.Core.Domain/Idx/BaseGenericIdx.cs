using Carpo.Core.Interface.Idx;

namespace Carpo.Core.Domain.Idx
{
    /// <summary>
    /// Classe para classe que irão representar um campo do tipo Idx
    /// </summary>
    public abstract class BaseGenericIdx
    {
        /// <summary>
        /// Listar as opções do campo do tipo Idx
        /// </summary>       
        public List<IIdx> GetListBaseIdc<TEntity>()
        {

            Type type = typeof(TEntity);
            var classIdx = type.GetNestedTypes();
            List<BaseIdx> listBase = new List<BaseIdx>();

            foreach (var idc in classIdx)
            {
                var value = (from fi in idc.GetFields()
                             where fi.Name == "IdxValue"
                             select fi.GetValue(null)).FirstOrDefault();

                var desc = (from fi in idc.GetFields()
                            where fi.Name == "IdxDescription"
                            select fi.GetValue(null)).FirstOrDefault();
                if (value != null && desc != null)
                {
                    listBase.Add(new BaseIdx { IdxValue = value.ToString(), IdxDescription = desc.ToString() });
                }
            }

            return listBase.ToList<IIdx>();
        }
    }
}
