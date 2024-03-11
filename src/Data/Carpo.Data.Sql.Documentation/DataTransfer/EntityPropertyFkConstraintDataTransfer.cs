using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Data.Sql.Documentation.DataTransfer
{
    /// <summary>
    /// Class for a property representing a foreign key constraint
    /// </summary>
    public class EntityPropertyFkConstraintDataTransfer
    {
        /// <summary>
        /// Name for the foreign key constraint
        /// </summary>
        public string ConstraintFkName { get; set; }

        /// <summary>
        /// Name for the reference table of the foreign key
        /// </summary>
        public string TableNameReference { get; set; }

        /// <summary>
        /// Name of the schema of the reference table of the foreign key
        /// </summary>
        public string SchemaTableNameReference { get; set; }

        /// <summary>
        /// Primary key name of the reference table of the foreign key, if different from the default Ide+[TABLE_NAME]
        /// </summary>
        public string PkNameTableReference { get; set; }

        /// <summary>
        /// Indicates if there is a name for the foreign key constraint
        /// </summary>
        public bool IsConstraintFkName { get { return !string.IsNullOrEmpty(ConstraintFkName); } }
    }

}
