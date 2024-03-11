namespace Carpo.Core.Domain.DomainDescription
{
    /// <summary>
    /// Informações da propriedade
    /// </summary>
    public class EntityPropertyDescriptionDataTransfer
    {
        /// <summary>
        /// Nome do campo
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Descrição do campo
        /// </summary>
        public string PropertyDescription { get; set; }
        /// <summary>
        /// Tipo do campo
        /// </summary>
        public string PropertyTypeName { get; set; }

        /// <summary>
        /// Nome da tabela de referência
        /// </summary>
        public string TableNameReference { get; set; }

        /// <summary>
        /// Identifica se é pra gerar a descrição neste campo
        /// </summary>
        public bool IsGenerateDescription { get; set; }

        /// <summary>
        /// Verifica se a propriedade é uma Pk
        /// </summary>
        public bool IsPropertyPk { get; set; }

        /// <summary>
        /// Verifica se a propriedade é uma Pk
        /// </summary>
        public bool IsPropertyFk
        {
            get
            {
                return DomainPropertyConstraintFk != null;
            }
        }

        /// <summary>
        /// DomainPropertyConstraintPk
        /// </summary>
        public EntityPropertyConstraintPkDataTransfer DomainPropertyConstraintPk { get; set; }

        /// <summary>
        /// DomainPropertyConstraintFk
        /// </summary>
        public EntityPropertyConstraintFkDataTransfer DomainPropertyConstraintFk { get; set; }

    }
}
