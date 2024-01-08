namespace Carpo.Core.Domain.DomainDescription
{
    /// <summary>
    /// Objeto de descrição da classe de domínio
    /// </summary>
    public class DomainClassDescription
    {
        /// <summary>
        /// Nome do Schema da tabela
        /// </summary>
        public string SchemaName { get; private set; }
        /// <summary>
        /// Nome da tabela
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// Descrição da tabela
        /// </summary>
        public string TableDescription { get; private set; }

        /// <summary>
        /// Identifica se a entidade é uma view
        /// </summary>
        public bool IsView { get; private set; }

        /// <summary>
        /// Lista de propriedades da classe de domínio
        /// </summary>
        public List<DomainPropertyDescription> ListDomainPropertyDescription { get; private set; }

        /// <summary>
        /// Construtor de criação da classe de descrição do domínio.
        /// </summary>        
        public DomainClassDescription(string schemaName, string tableName, string tableDescription, List<DomainPropertyDescription> listPropertyDescription, bool isView = false)
        {
            if (tableName.Length > 30)
            {
                throw new ArgumentException("O nome da tabela deve conter no máximo 30 caracteres.");
            }
            IsView = isView;
            SchemaName = schemaName;
            TableName = tableName;
            TableDescription = tableDescription;
            ListDomainPropertyDescription = listPropertyDescription ?? new List<DomainPropertyDescription>();
        }
    }
}
