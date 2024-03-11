namespace Carpo.Data.Sql.Documentation.DataTransfer
{
    /// <summary>
    /// Class for a property representing a primary key
    /// </summary>
    public class EntityPropertyConstraintPkDataTransfer
    {
        /// <summary>
        /// Name for the primary key constraint
        /// </summary>
        public string ConstraintNamePk { get; set; }

        /// <summary>
        /// Defines a name for the primary key
        /// </summary>
        public string NamePk { get; set; }

        /// <summary>
        /// Indicates if there is another name for the primary key
        /// </summary>
        public bool IsNameKey { get { return !string.IsNullOrEmpty(NamePk); } }

        /// <summary>
        /// Indicates if there is a name for the primary key constraint
        /// </summary>
        public bool IsConstraintNamePk { get { return !string.IsNullOrEmpty(ConstraintNamePk); } }
    }

}
