using Carpo.Data.Sql.Documentation.DataTransfer;
using System.Reflection;
using System.Xml;

namespace Carpo.Data.Sql.Documentation
{
    /// <summary>
    /// Class for capturing descriptions of domain classes and properties
    /// </summary>
    public static class EntityDocumentationCore
    {
        /// <summary>
        /// Constructs the DomainClassDescription class
        /// </summary>        
        public static EntityDescriptionDataTransfer GetDomainClassDescription(Type type, bool isLoadProperties = true)
        {
            if (!type.IsClass)
            {
                throw new ArgumentException("The entity type does not represent a class");
            }

            XmlElement xmlDomainClass = GetXmlDescriptionClass(type);
            string description = "";
            string entityName = type.Name;
            string groupName = "dbo";
            if (xmlDomainClass != null)
            {
                description = xmlDomainClass["summary"] == null ? "" : xmlDomainClass["summary"].InnerText.Trim();
                entityName = xmlDomainClass["entityName"] == null ? type.Name + "" : xmlDomainClass["entityName"].InnerText.Trim();
                groupName = xmlDomainClass["groupName"] == null ? "dbo" : xmlDomainClass["groupName"].InnerText.Trim(); 
            }
            bool isView = (xmlDomainClass != null && xmlDomainClass["isView"] != null);
            
            if (isView)
            {
                return new EntityDescriptionDataTransfer(groupName, entityName, description, new List<EntityPropertyDescriptionDataTransfer>(), isView);
            }

            return new EntityDescriptionDataTransfer(groupName, entityName, description, isLoadProperties ? GetListDomainPropertyDescription(type, entityName, groupName) : null);
        }

        /// <summary>
        /// Retrieves the list of domain property descriptions
        /// </summary>        
        public static List<EntityPropertyDescriptionDataTransfer> GetListDomainPropertyDescription(Type typeDomain, string tableName, string schemaName)
        {
            List<EntityPropertyDescriptionDataTransfer> listDomainPropertyDescription = new List<EntityPropertyDescriptionDataTransfer>();

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
                    string errorMessage = exc.InnerException != null ?
                                        exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message
                                        : exc.InnerException.Message
                                    : exc.Message;
                    exc = new Exception("ERROR: Property > " + prop.Name + " MSG: " + errorMessage);
                    throw exc;
                }
            }

            return listDomainPropertyDescription;
        }

        /// <summary>
        /// Checks if the property has the DataAnnotations KeyAttribute
        /// </summary>     
        public static bool IsPropertyPrimaryKey(PropertyInfo prop)
        {
            var keyAttribute = Attribute.GetCustomAttribute(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)) as System.ComponentModel.DataAnnotations.KeyAttribute;
            return (keyAttribute != null);
        }

        /// <summary>
        /// Retrieves the property description.
        /// </summary>        
        public static EntityPropertyDescriptionDataTransfer GetDomainPropertyDescription(PropertyInfo property, string tableName, string schemaName)
        {
            XmlElement xmlElement = GetXmlDescriptionProperty(property);

            return new EntityPropertyDescriptionDataTransfer
            {
                PropertyName = property.Name,
                PropertyTypeName = property.PropertyType.Name,
                PropertyDescription = xmlElement["summary"].InnerText.Trim(),
                IsGenerateDescription = (xmlElement["noDesc"] == null),
                IsPropertyPk = IsPropertyPrimaryKey(property),
                DomainPropertyPkConstraint = GetDomainPropertyConstraintPk(xmlElement),
                DomainPropertyFkConstraint = GetDomainPropertyConstraintFk(xmlElement, tableName, schemaName)
            };
        }

        /// <summary>
        /// Retrieves the primary key information if it exists. Returns NULL if it doesn't exist.
        /// </summary>        
        internal static EntityPropertyPkConstraintDataTransfer GetDomainPropertyConstraintPk(XmlElement xmlElement)
        {
            if (xmlElement["constraintPkName"] != null || xmlElement["keyName"] != null)
            {
                string pkName = (xmlElement["keyName"] == null) ? null : xmlElement["keyName"].InnerText.Trim();
                string constraintPkName = (xmlElement["constraintPkName"] == null) ? null : xmlElement["constraintPkName"].InnerText.Trim();

                return new EntityPropertyPkConstraintDataTransfer
                {
                    ConstraintNamePk = constraintPkName,
                    NamePk = pkName
                };
            }
            return null;
        }

        /// <summary>
        /// Retrieves the foreign key information if it exists. Returns NULL if it doesn't exist.
        /// </summary>        
        internal static EntityPropertyFkConstraintDataTransfer GetDomainPropertyConstraintFk(XmlElement xmlElement, string tableName, string schemaName)
        {
            EntityPropertyFkConstraintDataTransfer domainPropertyConstraintFk =
            (xmlElement["tableNameReference"] != null)
            ? new EntityPropertyFkConstraintDataTransfer
            {
                ConstraintFkName = xmlElement["constraintFkName"] == null
                                        ? "FK_" + tableName.Trim() + "_" + xmlElement["tableNameReference"].InnerText.Trim()
                                        : xmlElement["constraintFkName"].InnerText.Trim(),
                TableNameReference = xmlElement["tableNameReference"].InnerText.Trim(),
                PkNameTableReference = xmlElement["namePkTableReference"] == null
                                        ? xmlElement["tableNameReference"].InnerText.Trim()+"Id"
                                        : xmlElement["namePkTableReference"].InnerText.Trim(),
                SchemaTableNameReference = xmlElement["schemaTableNameReference"] == null
                                        ? schemaName
                                        : xmlElement["schemaTableNameReference"].InnerText.Trim()
            }
            : null;

            return domainPropertyConstraintFk;
        }

        /// <summary>
        /// Retrieves the XML node of the class summary, schemaTable, and tableName
        /// </summary>   
        internal static XmlElement GetXmlDescriptionClass(Type type)
        {
            // Prefix in type names is T
            return GetXmlFromName(type, 'T', "");
        }

        /// <summary>
        /// Retrieves the XML node of the property summary of the domain
        /// </summary>      
        internal static XmlElement GetXmlDescriptionProperty(MemberInfo memberInfo)
        {
            // First character [0] of member type is prefix character in the name in the XML
            return GetXmlFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name);
        }

        /// <summary>
        /// Retrieves the XML node of the object description
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
                throw new ArgumentException("Comment not assigned to property: " + type.Name);
            }

            return matchedElement;
        }

        /// <summary>
        /// Cache used to remember the xml file of the project
        /// </summary>
        internal static Dictionary<Assembly, XmlDocument> cache = new Dictionary<Assembly, XmlDocument>();

        /// <summary>
        /// Cache used to store exceptions.
        /// </summary>
        internal static Dictionary<Assembly, Exception> failCache = new Dictionary<Assembly, Exception>();

        /// <summary>
        /// Retrieves the xml documentation of the project from the storage cache.
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
        /// Retrieves the generated project's xml documentation.
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
