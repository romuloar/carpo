using System.Text;
using System.Xml.Serialization;

namespace Carpo.Core.Extension
{
    public static class StringCore
    {
        /// <summary>
        // Formats the string according to the specified mask
        // The hash character(#) is used to identify what will be replaced
        // Example 1: str = 1234567890, mask = (##) ####-####; Return = (12) 3456-7890
        // Example 2: str = 1234567890, mask = (##) ####-; Return = (12) 3456-7890
        // Example 3: str = 123456, mask = (##) ####-####; Return = (12) 3456-####     
        /// </summary>             
        /// <returns>String</returns>
        public static string Mask(this string str, string mascara)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(mascara)) return null;
            try
            {
                StringBuilder stringMascarada = new StringBuilder();

                List<string> listStr = str.Select(c => c.ToString()).ToList();

                foreach (char item in mascara)
                {
                    char nextCaractere;
                    if (item == '#')
                    {
                        if (listStr.Any())
                        {
                            nextCaractere = Convert.ToChar(listStr.FirstOrDefault());
                            listStr.RemoveRange(0, 1);
                        }
                        else
                        {
                            nextCaractere = item;
                        }
                    }
                    else
                    {
                        nextCaractere = item;
                    }
                    stringMascarada.Append(nextCaractere);
                }

                string valorMascarado = stringMascarada.ToString();
                if (listStr.Any())
                {
                    valorMascarado += String.Join("", listStr);
                }

                return valorMascarado;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Convert a string that is in xml format to an entity.
        /// </summary>
        /// <typeparam name="TEntity">Object you want to return</typeparam>
        /// <param name="xml">XML string representing the return object</param>        
        public static TEntity XmlToEntity<TEntity>(this string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                var serializador = new XmlSerializer(typeof(TEntity));
                return (TEntity)serializador.Deserialize(stringReader);
            }
        }
    }
}
