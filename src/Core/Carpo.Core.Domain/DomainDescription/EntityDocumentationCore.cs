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

            bool isView = (xmlDomainClass["isView"] != null);
            string description = xmlDomainClass["summary"] == null ? "" : xmlDomainClass["summary"].InnerText.Trim();
            string tableName = xmlDomainClass["nameEntity"] == null ? type.Name + "" : xmlDomainClass["nameEntity"].InnerText.Trim();
            string schemaName = xmlDomainClass["groupName"] == null ? "dbo" : xmlDomainClass["groupName"].InnerText.Trim();

            if (isView)
            {
                return new EntityDescriptionDataTransfer(schemaName, tableName, description, new List<EntityPropertyDescriptionDataTransfer>(), isView);
            }

            if (xmlDomainClass["summary"] == null)
            {
                throw new ArgumentException("The summary tag must be defined in the domain class");
            }

            if (xmlDomainClass["nameEntity"] == null)
            {
                throw new ArgumentException("The nameEntity tag must be defined in the domain class");
            }

            //if (xmlDomainClass["groupName"] == null)
            //{
            //    throw new ArgumentException("The schemaName tag must be defined in the domain class");
            //}

            return new EntityDescriptionDataTransfer(schemaName, tableName, description, isLoadProperties ? GetListDomainPropertyDescription(type, tableName, schemaName) : null);
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
                DomainPropertyConstraintPk = GetDomainPropertyConstraintPk(xmlElement),
                DomainPropertyConstraintFk = GetDomainPropertyConstraintFk(xmlElement, tableName, schemaName)
            };
        }

        /// <summary>
        /// Retrieves the primary key information if it exists. Returns NULL if it doesn't exist.
        /// </summary>        
        internal static EntityPropertyConstraintPkDataTransfer GetDomainPropertyConstraintPk(XmlElement xmlElement)
        {
            if (xmlElement["constraintNamePk"] != null || xmlElement["nameKey"] != null)
            {
                string namePk = (xmlElement["nameKey"] == null) ? null : xmlElement["nameKey"].InnerText.Trim();
                string constraintNamePk = (xmlElement["constraintNamePk"] == null) ? null : xmlElement["constraintNamePk"].InnerText.Trim();

                return new EntityPropertyConstraintPkDataTransfer
                {
                    ConstraintNamePk = constraintNamePk,
                    NamePk = namePk
                };
            }
            return null;
        }

        /// <summary>
        /// Retrieves the foreign key information if it exists. Returns NULL if it doesn't exist.
        /// </summary>        
        internal static EntityPropertyConstraintFkDataTransfer GetDomainPropertyConstraintFk(XmlElement xmlElement, string tableName, string schemaName)
        {
            EntityPropertyConstraintFkDataTransfer domainPropertyConstraintFk =
            (xmlElement["tableNameReference"] != null)
            ? new EntityPropertyConstraintFkDataTransfer
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
