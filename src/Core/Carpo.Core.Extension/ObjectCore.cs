using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;

namespace Carpo.Core.Extension
{
    public static class ObjectCore
    {
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static TEntity JsonToEntity<TEntity>(this string strJson) where TEntity : class
        {
            try
            {
#pragma warning disable CS8603 // Possible null reference return.
                return JsonSerializer.Deserialize<TEntity>(strJson);
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public static void CopyProperties<TObject>(this object fromObject, ref TObject toObject)
        {
            try
            {
                Type typeFrom = fromObject.GetType();
                Type typeTo = typeof(TObject);

                PropertyInfo[] listFieldFrom = typeFrom.GetProperties();
                PropertyInfo[] listFieldTo = typeTo.GetProperties();

                foreach (PropertyInfo fieldTo in listFieldTo)
                {
                    PropertyInfo fieldFrom = listFieldFrom.Where(p => p.Name == fieldTo.Name).FirstOrDefault();

                    if (fieldFrom != null)
                    {
                        if (fieldFrom.PropertyType == fieldTo.PropertyType)
                        {
                            var valueFrom = typeFrom.GetProperty(fieldFrom.Name).GetValue(fromObject, null);
                            typeTo.GetProperty(fieldTo.Name).SetValue(toObject, valueFrom, null);
                        }
                    }
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public static string ToXml(this object obj)
        {
            return ToXml(obj, obj.GetType());
        }

        public static string ToXml(this object obj, Type type)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(type);
                serializer.Serialize(stringwriter, obj);

                return stringwriter.ToString();
            }
        }
        
    }
}
