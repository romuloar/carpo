using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Data.Sql.Documentation.DataTransfer
{
    /// <summary>
    /// Property information
    /// </summary>
    public class EntityPropertyDescriptionDataTransfer
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Field description
        /// </summary>
        public string PropertyDescription { get; set; }
        /// <summary>
        /// Field type
        /// </summary>
        public string PropertyTypeName { get; set; }

        /// <summary>
        /// Name of the reference table
        /// </summary>
        public string TableNameReference { get; set; }

        /// <summary>
        /// Indicates whether to generate the description in this field
        /// </summary>
        public bool IsGenerateDescription { get; set; }

        /// <summary>
        /// Checks if the property is a PK
        /// </summary>
        public bool IsPropertyPk { get; set; }

        /// <summary>
        /// Checks if the property is a FK
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
