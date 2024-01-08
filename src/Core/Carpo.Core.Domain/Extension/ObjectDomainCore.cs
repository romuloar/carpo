using Carpo.Core.Domain.DomainDescription;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Core.Domain.Extension
{
    /// <summary>
    /// Métodos de extensão para classes de domínio
    /// </summary>
    public static class ObjectDomain
    {
        /// <summary>
        /// Lista as chaves identificadas como KeyAttribute 
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public static object[] GetListPrimaryKey<TDomain>(this TDomain entidade)
        {
            PropertyInfo[] props = typeof(TDomain).GetProperties();

            List<object> listObject = new List<object>();

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    KeyAttribute authAttr = attr as KeyAttribute;
                    if (authAttr != null)
                    {
                        var field = entidade.GetType().GetProperty(prop.Name).GetValue(entidade, null);
                        listObject.Add(field);
                    }
                }
            }

            return listObject.ToArray();
        }

        /// <summary>
        /// Converte uma lista de objeto domain para DataTable
        /// </summary>               
        public static DataTable ToDataTable<TEntity>(this IEnumerable<TEntity> data) where TEntity : BaseDomain
        {
            DomainClassDescription descriptionTable = DomainDescriptionCore.GetDomainClassDescription(typeof(TEntity));
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(TEntity));
            var table = new DataTable();

            List<string> listValidProperties = descriptionTable.ListDomainPropertyDescription.Where(p => p.IsGenerateDescription).Select(p => p.PropertyName).ToList();
            foreach (PropertyDescriptor prop in properties)
            {
                if (listValidProperties.Contains(prop.Name))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }

            foreach (TEntity item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (listValidProperties.Contains(prop.Name))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }
            return table;
        }

    }
}
