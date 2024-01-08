using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Carpo.Core.Domain.DomainDescription
{
    /// <summary>
    /// Classe para a captura das descrições das classes e propriedades de domínio
    /// </summary>
    public static class DomainDescriptionCore
    {
        /// <summary>
        /// Monta a classe DomainClassDescription
        /// </summary>        
        public static DomainClassDescription GetDomainClassDescription(Type type, bool isLoadProperties = true)
        {
            if (!type.IsClass)
            {
                throw new ArgumentException("O tipo da entidade não representa uma classe");
            }
            if (type.BaseType.Name != "BaseDomain")
            {
                throw new ArgumentException("É necessário que a classe herde de BaseDomain.");
            }

            XmlElement xmlDomainClass = GetXmlDescriptionClass(type);

            bool isView = (xmlDomainClass["isView"] != null);
            string description = xmlDomainClass["summary"] == null ? "" : xmlDomainClass["summary"].InnerText.Trim();
            string tableName = xmlDomainClass["nameEntity"] == null ? type.Name + "" : xmlDomainClass["nameEntity"].InnerText.Trim();
            string schemaName = xmlDomainClass["groupName"] == null ? "dbo" : xmlDomainClass["groupName"].InnerText.Trim();

            if (isView)
            {
                return new DomainClassDescription(schemaName, tableName, description, new List<DomainPropertyDescription>(), isView);
            }

            if (xmlDomainClass["summary"] == null)
            {
                throw new ArgumentException("É necessário definir a tag de comentário summary na classe de domínio");
            }

            if (xmlDomainClass["nameEntity"] == null)
            {
                throw new ArgumentException("É necessário definir a tag de comentário nameEntity na classe de domínio");
            }

            //if (xmlDomainClass["groupName"] == null)
            //{
            //    throw new ArgumentException("É necessário definir a tag de comentário schemaName na classe de domínio");
            //}

            return new DomainClassDescription(schemaName, tableName, description, isLoadProperties ? GetListDomainPropertyDescription(type, tableName, schemaName) : null);
        }

        /// <summary>
        /// Busca a lista de descrições das propriedades de domínio
        /// </summary>        
        public static List<DomainPropertyDescription> GetListDomainPropertyDescription(Type typeDomain, string tableName, string schemaName)
        {
            List<DomainPropertyDescription> listDomainPropertyDescription = new List<DomainPropertyDescription>();

            foreach (var prop in typeDomain.GetProperties())
            {
                try
                {
                    var notMappedAttribute = Attribute.GetCustomAttribute(prop, typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) as System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute;

                    if (notMappedAttribute == null && !prop.Name.StartsWith("List"))
                    {
                        listDomainPropertyDescription.Add(GetDomainPropertyDescription(prop, tableName, schemaName));
                    }
                }
                catch (Exception exc)
                {
                    string msgErro = exc.InnerException != null ?
                                        exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message
                                        : exc.InnerException.Message
                                    : exc.Message;
                    exc = new Exception("ERROR: Property > " + prop.Name + " MSG: " + msgErro);
                    throw exc;
                }
            }

            return listDomainPropertyDescription;
        }

        /// <summary>
        /// Verifica se a propriedade tem a DataAnnotations KeyAttribute
        /// </summary>     
        public static bool IsPropertyPrimaryKey(PropertyInfo prop)
        {
            var keyAttribute = Attribute.GetCustomAttribute(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)) as System.ComponentModel.DataAnnotations.KeyAttribute;
            return (keyAttribute != null);
        }

        /// <summary>
        /// Busca a descrição da propriedade.
        /// </summary>        
        public static DomainPropertyDescription GetDomainPropertyDescription(PropertyInfo property, string tableName, string schemaName)
        {
            XmlElement xmlElement = GetXmlDescriptionProperty(property);

            return new DomainPropertyDescription
            {
                PropertyName = property.Name,
                PropertyTypeName = property.PropertyType.Name,
                PropertyDescription = xmlElement["summary"].InnerText.Trim(),
                IsGenerateDescription = (xmlElement["noDesc"] == null),
                IsPropertyPk = IsPropertyPrimaryKey(property),
                DomainPropertyConstraintPk = GetDomainPropertyConstraintPk(xmlElement),
                DomainPropertyConstraintFk = GetDomainPropertyConstraintFk(xmlElement, tableName, schemaName)
            };
        }

        /// <summary>
        /// Busca as informações de chave primária caso existam. Caso não exista retorna NULL
        /// </summary>        
        internal static DomainPropertyConstraintPk GetDomainPropertyConstraintPk(XmlElement xmlElement)
        {
            if (xmlElement["constraintNamePk"] != null || xmlElement["nameKey"] != null)
            {
                string namePk = (xmlElement["nameKey"] == null) ? null : xmlElement["nameKey"].InnerText.Trim();
                string constraintNamePk = (xmlElement["constraintNamePk"] == null) ? null : xmlElement["constraintNamePk"].InnerText.Trim();

                return new DomainPropertyConstraintPk
                {
                    ConstraintNamePk = constraintNamePk,
                    NamePk = namePk
                };
            }
            return null;
        }

        /// <summary>
        /// Busca as informações de chave estrangeira caso exista. Caso não exista retorna NULL
        /// </summary>        
        internal static DomainPropertyConstraintFk GetDomainPropertyConstraintFk(XmlElement xmlElement, string tableName, string schemaName)
        {
            DomainPropertyConstraintFk domainPropertyConstraintFk =
            (xmlElement["tableNameReference"] != null)
            ? new DomainPropertyConstraintFk
            {
                ConstraintNameFk = xmlElement["constraintNameFk"] == null
                                        ? "FK_" + tableName.Trim() + "_" + xmlElement["tableNameReference"].InnerText.Trim()
                                        : xmlElement["constraintNameFk"].InnerText.Trim(),
                TableNameReference = xmlElement["tableNameReference"].InnerText.Trim(),
                NamePkTableReference = xmlElement["namePkTableReference"] == null
                                        ? "Ide" + xmlElement["tableNameReference"].InnerText.Trim()
                                        : xmlElement["namePkTableReference"].InnerText.Trim(),
                SchemaTableNameReference = xmlElement["schemaTableNameReference"] == null
                                        ? schemaName
                                        : xmlElement["schemaTableNameReference"].InnerText.Trim()
            }
            : null;

            return domainPropertyConstraintFk;
        }

        /// <summary>
        /// Busca o nó xml do summary, schemaTable e tableName da classe de domínio
        /// </summary>   
        internal static XmlElement GetXmlDescriptionClass(Type type)
        {
            // Prefix in type names is T
            return GetXmlFromName(type, 'T', "");
        }

        /// <summary>
        /// Busca o nó xml do summary da propriedade do domínio
        /// </summary>      
        internal static XmlElement GetXmlDescriptionProperty(MemberInfo memberInfo)
        {
            // First character [0] of member type is prefix character in the name in the XML
            return GetXmlFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name);
        }

        /// <summary>
        /// Busca o nó xml da descrição do objeto
        /// </summary>                
        internal static XmlElement GetXmlFromName(Type type, char prefix, string name)
        {
            string fullName;
            if (String.IsNullOrEmpty(name))
            {
                fullName = prefix + ":" + type.FullName;
            }
            else
            {
                fullName = prefix + ":" + type.FullName + "." + name;
            }

            XmlDocument xmlDocument = GetXmlDocumentFromAssembly(type.Assembly);

            XmlElement matchedElement = null;

            foreach (XmlElement xmlElement in xmlDocument["doc"]["members"])
            {
                if (xmlElement.Attributes["name"].Value.Equals(fullName))
                {
                    if (matchedElement != null)
                    {
                        throw new ArgumentException("Multiple matches to query");
                    }

                    matchedElement = xmlElement;
                }
            }

            if (matchedElement == null)
            {
                throw new ArgumentException("Cometário não atribuído a propriedade: " + type.Name);
            }

            return matchedElement;
        }

        /// <summary>
        /// Cache utilizado para lembrar do arquivo xml do projeto
        /// </summary>
        internal static Dictionary<Assembly, XmlDocument> cache = new Dictionary<Assembly, XmlDocument>();

        /// <summary>
        /// Cache utilizado para armazenar exceções.
        /// </summary>
        internal static Dictionary<Assembly, Exception> failCache = new Dictionary<Assembly, Exception>();

        /// <summary>
        /// Busca a documentação xml do projeto do cache de armazenamento.
        /// </summary>      
        internal static XmlDocument GetXmlDocumentFromAssembly(Assembly assembly)
        {
            if (failCache.ContainsKey(assembly))
            {
                throw failCache[assembly];
            }
            try
            {
                if (!cache.ContainsKey(assembly))
                {
                    cache[assembly] = GetXmlDocumentFromAssemblyNonCached(assembly);
                }

                return cache[assembly];
            }
            catch (Exception exception)
            {
                failCache[assembly] = exception;
                throw exception;
            }
        }

        /// <summary>
        /// Busca a documentação xml do projeto gerado.
        /// </summary>        
        internal static XmlDocument GetXmlDocumentFromAssemblyNonCached(Assembly assembly)
        {
            string assemblyFilename = assembly.CodeBase;

            const string prefix = "file:///";

            if (assemblyFilename.StartsWith(prefix))
            {
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml"));
                }
                catch (FileNotFoundException exception)
                {
                    throw new ArgumentException(exception.Message + "XML documentation not present (make sure it is turned on in project properties when building)");
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(streamReader);
                return xmlDocument;
            }
            else
            {
                throw new ArgumentException("Could not ascertain assembly filename");
            }
        }
    }
}
