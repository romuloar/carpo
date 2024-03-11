namespace Carpo.Core.Domain.DomainDescription
{
    /// <summary>
    /// Class for a property representing a foreign key constraint
    /// </summary>
    public class EntityPropertyConstraintFkDataTransfer
    {
        /// <summary>
        /// Name for the foreign key constraint
        /// </summary>
        public string ConstraintNameFk { get; set; }

        /// <summary>
        /// Name for the reference table of the foreign key
        /// </summary>
        public string TableNameReference { get; set; }

        /// <summary>
        /// Name of the schema of the reference table of the foreign key
        /// </summary>
        public string SchemaTableNameReference { get; set; }

        /// <summary>
        /// Name of the PK of the reference table of the foreign key, if different from the default Ide+[TABLE_NAME]
        /// </summary>
        public string NamePkTableReference { get; set; }

        /// <summary>
        /// Indicates if there is a name for the foreign key constraint
        /// </summary>
        public bool IsConstraintNameFk { get { return !string.IsNullOrEmpty(ConstraintNameFk); } }
    }

}
