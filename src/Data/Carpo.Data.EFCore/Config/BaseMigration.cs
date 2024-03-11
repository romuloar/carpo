using Carpo.Core.Domain;
using Carpo.Core.Domain.Attributes;
using Carpo.Core.Domain.Attributes.Sql;
using Carpo.Core.Interface.Idx;
using Carpo.Data.Sql.Documentation;
using Carpo.Data.Sql.Documentation.DataTransfer;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace Carpo.Data.EFCore.Config
{
    /// <summary>
    /// Class to generate database documentation
    /// </summary>
    public class BaseMigration : Migration
    {
        /// <summary>
        /// 
        /// </summary>
        internal MigrationBuilder? _migrationBuilder;

        //public BaseMigration()
        //{
        //    _migrationBuilder = new MigrationBuilder(null);
        //}

        /// <summary>
        /// To create custom database scripts
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _migrationBuilder = migrationBuilder;
        }

        /// <summary>
        /// Generate the script for creating the Index, unique fields and table and column descriptions.
        /// </summary>
        public void GenerateCustomScript(MigrationBuilder migrationBuilder)
        {
            _migrationBuilder = migrationBuilder;
            if (_migrationBuilder != null)
            {
                var listType = GetListDomainType();
                if (listType.Any())
                {
                    _migrationBuilder.Sql("-- Data dictionary creation");
                    _migrationBuilder.Sql("-- Creation of check constraints for Idx fields");
                    _migrationBuilder.Sql("-- Creation of unique constraints");
                    _migrationBuilder.Sql("-- Correction of primary key constraint names");
                    _migrationBuilder.Sql("-- Correction of all foreign key constraint names");
                }
                foreach (var mappingClass in listType)
                {
                    var descriptionTable = EntityDocumentationCore.GetDomainClassDescription(mappingClass);
                    if (descriptionTable.IsView)
                    {
                        continue;
                    }
                    _migrationBuilder.Sql("--------------------------------->>Table: [" + descriptionTable.GroupName + "].[" + descriptionTable.EntityName + "]");
                    _migrationBuilder.Sql(MountTableDescriptionScript(descriptionTable));
                    AddDescriptionScript(mappingClass, descriptionTable);
                    AddCheckIdxScript(mappingClass, descriptionTable);
                    AddUniqueScript(mappingClass, descriptionTable);
                    AddFieldDefaultValueScript(mappingClass, descriptionTable);

                    //AlterConstraintPk(descriptionTable);
                    _migrationBuilder.Sql("--------------------------------->>END");
                }

                foreach (var mappingClass in listType)
                {
                    var descriptionTable = EntityDocumentationCore.GetDomainClassDescription(mappingClass);
                    if (descriptionTable.IsView)
                    {
                        continue;
                    }
                    AlterConstraintFk(descriptionTable);
                }
                _migrationBuilder.Sql("--------------------------------->>END SCRIPT ");
            }

        }

        /// <summary>
        /// Add the domain creation script for each Idx
        /// </summary>        
        internal void AddCheckIdxScript(Type typeDomain, EntityDescriptionDataTransfer descriptionTable)
        {
            foreach (var prop in typeDomain.GetProperties())
            {
                var idx = Attribute.GetCustomAttribute(prop, typeof(IdxAttribute)) as IdxAttribute;
                if (idx != null && idx.ListIdx.Any())
                {
                    _migrationBuilder.Sql(MountCheckIdcScript(prop.Name, descriptionTable, idx.ListIdx));
                }
            }
        }

        /// <summary>
        /// Add the constraints change script (Pk and Fk), if any one.
        /// </summary>        
        internal void AlterConstraintPk(EntityDescriptionDataTransfer descriptionTable)
        {
            foreach (var prop in descriptionTable.ListDomainPropertyDescription)
            {
                if (prop.IsPropertyPk)
                {
                    _migrationBuilder.Sql("--->>ALTER CONSTRAINT PRIMARY KEY");
                    AlterConstraintPkScript(descriptionTable, prop);
                }
            }
        }

        /// <summary>
        /// Add constraints change scripts (Pk and Fk), if available
        /// </summary>        
        internal void AlterConstraintFk(EntityDescriptionDataTransfer descriptionTable)
        {
            if (descriptionTable.ListDomainPropertyDescription.Any(f => f.IsPropertyFk))
            {
                _migrationBuilder.Sql("--->>ALTER CONSTRAINT FOREIGN KEY [" + descriptionTable.GroupName + "].[" + descriptionTable.EntityName + "]");
            }
            foreach (var prop in descriptionTable.ListDomainPropertyDescription)
            {
                if (prop.IsPropertyFk)
                {
                    AddConstraintFkScript(descriptionTable, prop);
                }
            }
        }

        /// <summary>
        /// Add the Domain class property description script
        /// </summary>        
        internal void AddDescriptionScript(Type typeDomain, EntityDescriptionDataTransfer descriptionTable)
        {
            foreach (var propertyDescription in descriptionTable.ListDomainPropertyDescription.Where(p => p.IsGenerateDescription).ToList())
            {
                string columnName = (propertyDescription.DomainPropertyPkConstraint != null && propertyDescription.DomainPropertyPkConstraint.IsKeyName)
                                        ? propertyDescription.DomainPropertyPkConstraint.NamePk
                                        : propertyDescription.PropertyName;

                _migrationBuilder.Sql(MountDescriptionScript(propertyDescription.PropertyDescription, columnName, descriptionTable));
            }
        }

        /// <summary>
        /// AddFieldDefaultValueScript 
        /// </summary>
        /// <param name="typeDomain"></param>
        /// <param name="descriptionTable"></param>
        internal void AddFieldDefaultValueScript(Type typeDomain, EntityDescriptionDataTransfer descriptionTable)
        {
            foreach (var prop in typeDomain.GetProperties())
            {
                var fieldDefaultValue = Attribute.GetCustomAttribute(prop, typeof(FieldDefaultValueAttribute)) as FieldDefaultValueAttribute;
                if (fieldDefaultValue != null)
                {
                    var value = fieldDefaultValue.Value;
                    FieldValueType typeValue = fieldDefaultValue.FieldDefaultValueType;
                    string customValue = FieldValueType.String == typeValue ? "'" + value + "'" : value.ToString();

                    _migrationBuilder.Sql(MountFieldDefaultValueScript(descriptionTable, prop.Name, customValue));
                }
            }
        }

        /// <summary>
        /// Mount Field Default Value Script
        /// </summary>  
        internal string MountFieldDefaultValueScript(EntityDescriptionDataTransfer domainDescription, string columnName, string customValue)
        {
            return string.Format(@"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT DF_{1}_{2} DEFAULT ({3}) FOR [{2}]"
                                    , domainDescription.GroupName
                                    , domainDescription.EntityName
                                    , columnName
                                    , customValue);
        }

        /// <summary>
        /// Add the unique constraint to the script, if there is a field marked with the Unique attribute
        /// </summary>        
        internal void AddUniqueScript(Type typeDomain, EntityDescriptionDataTransfer descriptionTable)
        {
            List<string> listColumnName = new List<string>();
            foreach (var prop in typeDomain.GetProperties())
            {
                var descriptionAttribute = Attribute.GetCustomAttribute(prop, typeof(UniqueAttribute)) as UniqueAttribute;
                if (descriptionAttribute != null)
                {
                    listColumnName.Add(prop.Name);
                }
            }

            if (listColumnName.Any())
            {
                _migrationBuilder.Sql(MountUniqueScript(descriptionTable, listColumnName));
            }
        }

        /// <summary>
        /// Create the unique fields script
        /// </summary>  
        internal string MountUniqueScript(EntityDescriptionDataTransfer domainDescription, List<string> listColumnName)
        {
            string columns = string.Join(",", listColumnName);

            return string.Format(@"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT UK_{1}_{2} UNIQUE ({3});"
                                    , domainDescription.GroupName
                                    , domainDescription.EntityName
                                    , columns.Replace(",", "")
                                    , columns);
        }

        /// <summary>
        /// Create the check script for the Idc fields
        /// </summary>  
        internal string MountCheckIdcScript(string columnName, EntityDescriptionDataTransfer domainDescription, List<IIdx> listIdc)
        {
            string opcoes = string.Join(",", listIdc.Select(v => v.IdxValue).ToArray()).Replace(",", "','");

            return string.Format(@"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT CK_{1}_{2} CHECK ({2} IN ('{3}'));"
                                    , domainDescription.GroupName
                                    , domainDescription.EntityName
                                    , columnName
                                    , opcoes);
        }

        /// <summary>
        /// Create the column description script
        /// </summary>  
        internal string MountDescriptionScript(string descriptionColumn, string columnName, EntityDescriptionDataTransfer domainDescription)
        {
            return string.Format(@"EXEC sp_addextendedproperty 'MS_Description', '{0}','schema', '{1}', 'table', '{2}', 'column', '{3}';"
                                    , descriptionColumn
                                    , domainDescription.GroupName
                                    , domainDescription.EntityName
                                    , columnName);
        }

        /// <summary>
        /// List domains of type IValidationDomain
        /// </summary>        
        internal List<Type> GetListDomainType()
        {
            var baseDomainType = typeof(BaseDomain);
            var directSubclasses = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var typesInAssembly = assembly.GetExportedTypes()
                        .Where(type => type.BaseType == baseDomainType && type != baseDomainType);

                    directSubclasses.AddRange(typesInAssembly);
                }
                catch (NotSupportedException)
                {
                    continue;
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderExceptions = ex.LoaderExceptions;
                }
            }

            return directSubclasses;
        }

        /// <summary>
        /// Create the table description script
        /// </summary>        
        internal string MountTableDescriptionScript(EntityDescriptionDataTransfer domainDescription)
        {
            return string.Format(@"EXEC sp_addextendedproperty 'MS_Description','{0}', 'schema', '{1}', 'table', '{2}';"
                                    , domainDescription.EntityDescription
                                    , domainDescription.GroupName
                                    , domainDescription.EntityName);
        }

        /// <summary>
        /// Change the name of the primary key field constraint
        /// </summary>        
        internal void AlterConstraintPkScript(EntityDescriptionDataTransfer domainDescription, EntityPropertyDescriptionDataTransfer EntityPropertyDescriptionDataTransfer)
        {
            string namePk =
                (EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint != null && EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint.IsKeyName)
                ? EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint.NamePk
                : domainDescription.EntityName + "_Id";

            string nameConstraintPk =
                (EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint != null && EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint.IsPkConstraintName)
                ? EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint.ConstraintNamePk
                : "PK_" + domainDescription.EntityName;

            string schemaTable = string.Format(@"{0}.{1}"
                                                , domainDescription.GroupName
                                                , domainDescription.EntityName);
            _migrationBuilder.DropPrimaryKey(name: nameConstraintPk, table: domainDescription.EntityName, schema: schemaTable);
            _migrationBuilder.AddPrimaryKey(name: nameConstraintPk, table: domainDescription.EntityName, column: namePk);
        }

        /// <summary>
        /// Change the name of the foreign key field constraint
        /// </summary>        
        internal void AddConstraintFkScript(EntityDescriptionDataTransfer domainDescription, EntityPropertyDescriptionDataTransfer EntityPropertyDescriptionDataTransfer)
        {
            string nameColumnFk;
            string nameReferenceFk = EntityPropertyDescriptionDataTransfer.DomainPropertyFkConstraint.PkNameTableReference;

            if (EntityPropertyDescriptionDataTransfer.IsPropertyPk)
            {
                nameColumnFk = (EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint != null && EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint.IsKeyName)
                                        ? EntityPropertyDescriptionDataTransfer.DomainPropertyPkConstraint.NamePk
                                        : domainDescription.EntityName + "_Id";
            }
            else
            {
                nameColumnFk = EntityPropertyDescriptionDataTransfer.PropertyName;
            }

            string schemaTable = string.Format(@"[{0}].{1}"
                                    , domainDescription.GroupName
                                    , domainDescription.EntityName);

            string principalTable = string.Format(@"[{0}].{1}"
                                    , EntityPropertyDescriptionDataTransfer.DomainPropertyFkConstraint.SchemaTableNameReference
                                    , EntityPropertyDescriptionDataTransfer.DomainPropertyFkConstraint.TableNameReference);

            _migrationBuilder.AddForeignKey(
                name: nameReferenceFk,
                table: domainDescription.EntityName,
                columns: new string[] { nameColumnFk },
                principalTable: principalTable,
                schema: schemaTable);

        }
    }
}
