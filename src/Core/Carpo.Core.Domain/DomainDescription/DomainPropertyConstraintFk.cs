namespace Carpo.Core.Domain.DomainDescription
{
    /// <summary>
    /// Classe para propriedade que representa uma chave estrangeira
    /// </summary>
    public class DomainPropertyConstraintFk
    {
        /// <summary>
        /// Nome para a "constraint" de chave estrangeira
        /// </summary>
        public string ConstraintNameFk { get; set; }

        /// <summary>
        /// Nome para a tabela de referência da chave estrangeira
        /// </summary>
        public string TableNameReference { get; set; }

        /// <summary>
        /// Nome do schema da tabela de referência da chave estrangeira
        /// </summary>
        public string SchemaTableNameReference { get; set; }

        /// <summary>
        /// Nome da PK da tabela de referência da chave estrangeira, caso seja diferente do padrão Ide+[NOME_DA_TABELA]
        /// </summary>
        public string NamePkTableReference { get; set; }

        /// <summary>
        /// Identifica se existe nome para a "constraint" de chave estrangeira
        /// </summary>
        public bool IsConstraintNameFk { get { return !string.IsNullOrEmpty(ConstraintNameFk); } }

    }
}
