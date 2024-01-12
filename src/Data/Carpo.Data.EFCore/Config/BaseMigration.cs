using Carpo.Core.Domain;
using Carpo.Core.Domain.Attributes;
using Carpo.Core.Domain.Attributes.Sql;
using Carpo.Core.Domain.DomainDescription;
using Carpo.Core.Interface.Idx;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Carpo.Data.EFCore.Config
{
    /// <summary>
    /// Classe base para a geração de script
    /// </summary>
    public class BaseMigration : Migration
    {
        internal MigrationBuilder _migrationBuilder;

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
        /// Gera o script de crição dos campos Idx, unique e as descrições de tabelas e colunas.
        /// </summary>
        public void GenerateScriptCustom(MigrationBuilder migrationBuilder)
        {
            _migrationBuilder = migrationBuilder;
            if(_migrationBuilder != null)
            {
                var listType = GetListDomainType();
                if (listType.Any())
                {
                    _migrationBuilder.Sql("--Criação do dicionário de dados");
                    _migrationBuilder.Sql("--Criação das constraints check para os campos Idx");
                    _migrationBuilder.Sql("--Criação das constraints unique");
                    _migrationBuilder.Sql("--Correção dos nomes das constraints primary key");
                    _migrationBuilder.Sql("--Correção dos nomes de todas as constraints foreign keys");
                }
                foreach (var mappingClass in listType)
                {
                    DomainClassDescription descriptionTable = DomainDescriptionCore.GetDomainClassDescription(mappingClass);
                    if (descriptionTable.IsView)
                    {
                        continue;
                    }
                    _migrationBuilder.Sql("--------------------------------->>Tabela: [" + descriptionTable.SchemaName + "].[" + descriptionTable.TableName + "]");
                    _migrationBuilder.Sql(MountTableDescriptionScript(descriptionTable));
                    AddDescriptionScript(mappingClass, descriptionTable);
                    AddCheckIdxScript(mappingClass, descriptionTable);
                    AddUniqueScript(mappingClass, descriptionTable);
                    AddFieldDefaultValueScript(mappingClass, descriptionTable);

                    //AlterConstraintPk(descriptionTable);
                    _migrationBuilder.Sql("--------------------------------->>Fim");
                }

                foreach (var mappingClass in listType)
                {
                    DomainClassDescription descriptionTable = DomainDescriptionCore.GetDomainClassDescription(mappingClass);
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
        /// Adiciona os script de criação dos domínios de cada Idx
        /// </summary>        
        internal void AddCheckIdxScript(Type typeDomain, DomainClassDescription descriptionTable)
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
        /// Adiciona os script de alteração de constraints (Pk e Fk), caso exista
        /// </summary>        
        internal void AlterConstraintPk(DomainClassDescription descriptionTable)
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
        /// Adiciona os script de alteração de constraints (Pk e Fk), caso exista
        /// </summary>        
        internal void AlterConstraintFk(DomainClassDescription descriptionTable)
        {
            if (descriptionTable.ListDomainPropertyDescription.Any(f => f.IsPropertyFk))
            {
                _migrationBuilder.Sql("--->>ALTER CONSTRAINT FOREIGN KEY [" + descriptionTable.SchemaName + "].[" + descriptionTable.TableName + "]");
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
        /// Adiciona os script de descrição das propriedades da classe Domain
        /// </summary>        
        internal void AddDescriptionScript(Type typeDomain, DomainClassDescription descriptionTable)
        {
            foreach (var propertyDescription in descriptionTable.ListDomainPropertyDescription.Where(p => p.IsGenerateDescription).ToList())
            {
                string columnName = (propertyDescription.DomainPropertyConstraintPk != null && propertyDescription.DomainPropertyConstraintPk.IsNameKey)
                                        ? propertyDescription.DomainPropertyConstraintPk.NamePk
                                        : propertyDescription.PropertyName;

                _migrationBuilder.Sql(MountDescriptionScript(propertyDescription.PropertyDescription, columnName, descriptionTable));
            }
        }

        /// <summary>
        /// AddFieldDefaultValueScript 
        /// </summary>
        /// <param name="typeDomain"></param>
        /// <param name="descriptionTable"></param>
        internal void AddFieldDefaultValueScript(Type typeDomain, DomainClassDescription descriptionTable)
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
        internal string MountFieldDefaultValueScript(DomainClassDescription domainDescription, string columnName, string customValue)
        {
            return string.Format(@"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT DF_{1}_{2} DEFAULT ({3}) FOR [{2}]"
                                    , domainDescription.SchemaName
                                    , domainDescription.TableName
                                    , columnName
                                    , customValue);
        }

        /// <summary>
        /// Adiciona ao script a constraint unique, caso exista algum campo marcado com o atributo Unique 
        /// </summary>        
        internal void AddUniqueScript(Type typeDomain, DomainClassDescription descriptionTable)
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
        /// Monta o script dos campos unique
        /// </summary>  
        internal string MountUniqueScript(DomainClassDescription domainDescription, List<string> listColumnName)
        {
            string columns = string.Join(",", listColumnName);

            return string.Format(@"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT UK_{1}_{2} UNIQUE ({3});"
                                    , domainDescription.SchemaName
                                    , domainDescription.TableName
                                    , columns.Replace(",", "")
                                    , columns);
        }

        /// <summary>
        /// Monta o script de check para os campos Idc
        /// </summary>  
        internal string MountCheckIdcScript(string columnName, DomainClassDescription domainDescription, List<IIdx> listIdc)
        {
            string opcoes = string.Join(",", listIdc.Select(v => v.IdxValue).ToArray()).Replace(",", "','");

            return string.Format(@"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT CK_{1}_{2} CHECK ({2} IN ('{3}'));"
                                    , domainDescription.SchemaName
                                    , domainDescription.TableName
                                    , columnName
                                    , opcoes);
        }

        /// <summary>
        /// Monta o script de descrição da coluna
        /// </summary>  
        internal string MountDescriptionScript(string descriptionColumn, string columnName, DomainClassDescription domainDescription)
        {
            return string.Format(@"EXEC sp_addextendedproperty 'MS_Description', '{0}','schema', '{1}', 'table', '{2}', 'column', '{3}';"
                                    , descriptionColumn
                                    , domainDescription.SchemaName
                                    , domainDescription.TableName
                                    , columnName);
        }

        /// <summary>
        /// Lista os domínios do tipo IValidationDomain
        /// </summary>        
        internal List<Type> GetListDomainType()
        {
            var type = typeof(BaseDomain);
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && p.Name != "BaseDomain").ToList();
        }

        /// <summary>
        /// Monta o script de descrição da tabela
        /// </summary>        
        internal string MountTableDescriptionScript(DomainClassDescription domainDescription)
        {
            return string.Format(@"EXEC sp_addextendedproperty 'MS_Description','{0}', 'schema', '{1}', 'table', '{2}';"
                                    , domainDescription.TableDescription
                                    , domainDescription.SchemaName
                                    , domainDescription.TableName);
        }

        /// <summary>
        /// Alterra o nome da constraint do campo de chave primária
        /// </summary>        
        internal void AlterConstraintPkScript(DomainClassDescription domainDescription, DomainPropertyDescription domainPropertyDescription)
        {
            string namePk =
                (domainPropertyDescription.DomainPropertyConstraintPk != null && domainPropertyDescription.DomainPropertyConstraintPk.IsNameKey)
                ? domainPropertyDescription.DomainPropertyConstraintPk.NamePk
                : domainDescription.TableName + "_Id";

            string nameConstraintPk =
                (domainPropertyDescription.DomainPropertyConstraintPk != null && domainPropertyDescription.DomainPropertyConstraintPk.IsConstraintNamePk)
                ? domainPropertyDescription.DomainPropertyConstraintPk.ConstraintNamePk
                : "PK_" + domainDescription.TableName;

            string schemaTable = string.Format(@"{0}.{1}"
                                                , domainDescription.SchemaName
                                                , domainDescription.TableName);
            _migrationBuilder.DropPrimaryKey(name: nameConstraintPk, table: domainDescription.TableName, schema:schemaTable);
            _migrationBuilder.AddPrimaryKey(name: nameConstraintPk, table: domainDescription.TableName, column: namePk);
        }

        /// <summary>
        /// Altera o nome da constraint do campo de chave estrangeira
        /// </summary>        
        internal void AddConstraintFkScript(DomainClassDescription domainDescription, DomainPropertyDescription domainPropertyDescription)
        {
            string nameColumnFk;
            string nameReferenceFk = domainPropertyDescription.DomainPropertyConstraintFk.NamePkTableReference;

            if (domainPropertyDescription.IsPropertyPk)
            {
                nameColumnFk = (domainPropertyDescription.DomainPropertyConstraintPk != null && domainPropertyDescription.DomainPropertyConstraintPk.IsNameKey)
                                        ? domainPropertyDescription.DomainPropertyConstraintPk.NamePk
                                        : domainDescription.TableName + "_Id";
            }
            else
            {
                nameColumnFk = domainPropertyDescription.PropertyName;
            }

            string schemaTable = string.Format(@"[{0}].{1}"
                                    , domainDescription.SchemaName
                                    , domainDescription.TableName);

            string principalTable = string.Format(@"[{0}].{1}"
                                    , domainPropertyDescription.DomainPropertyConstraintFk.SchemaTableNameReference
                                    , domainPropertyDescription.DomainPropertyConstraintFk.TableNameReference);
            
            _migrationBuilder.AddForeignKey(
                name : nameReferenceFk,
                table: domainDescription.TableName,
                columns: new string[] { nameColumnFk },
                principalTable: principalTable,
                schema : schemaTable);

            //_migrationBuilder.AddForeignKey(
            //        schemaTable,
            //        nameColumnFk,
            //        principalTable,
            //        nameReferenceFk,
            //        false,
            //        domainPropertyDescription.DomainPropertyConstraintFk.ConstraintNameFk);
        }
    }
}
