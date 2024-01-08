namespace Carpo.Core.Domain.DomainDescription
{
    /// <summary>
    /// Classe para propriedade que representa uma chave primária
    /// </summary>
    public class DomainPropertyConstraintPk
    {
        /// <summary>
        /// Nome para a "constraint" de chave primária
        /// </summary>
        public string ConstraintNamePk { get; set; }

        /// <summary>
        /// Define um nome para a chave primária
        /// </summary>
        public string NamePk { get; set; }

        /// <summary>
        /// Identifica se existe outro nome para a chave primária
        /// </summary>
        public bool IsNameKey { get { return !string.IsNullOrEmpty(NamePk); } }

        /// <summary>
        /// Identifica se existe nome para a "constraint" de chave primária
        /// </summary>
        public bool IsConstraintNamePk { get { return !string.IsNullOrEmpty(ConstraintNamePk); } }

    }
}
