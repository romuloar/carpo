namespace Carpo.Data.Sql.Documentation.DataTransfer
{
    /// <summary>
    /// Object description of the domain class
    /// </summary>
    public class EntityDescriptionDataTransfer
    {
        /// <summary>
        /// Name of the table's schema
        /// </summary>
        public string GroupName { get; private set; }
        /// <summary>
        /// Table name
        /// </summary>
        public string EntityName { get; private set; }
        /// <summary>
        /// Table description
        /// </summary>
        public string EntityDescription { get; private set; }

        /// <summary>
        /// Indicates whether the entity is a view
        /// </summary>
        public bool IsView { get; private set; }

        /// <summary>
        /// List of properties of the domain class
        /// </summary>
        public List<EntityPropertyDescriptionDataTransfer> ListDomainPropertyDescription { get; private set; }

        /// <summary>
        /// Constructor for creating the domain description class.
        /// </summary>        
        public EntityDescriptionDataTransfer(string groupName, string entityName, string entityDescription, List<EntityPropertyDescriptionDataTransfer> listPropertyDescription, bool isView = false)
        {
            if (entityName.Length > 30)
            {
                throw new ArgumentException("The table name must contain a maximum of 30 characters.");
            }
            IsView = isView;
            GroupName = groupName;
            EntityName = entityName;
            EntityDescription = entityDescription;
            ListDomainPropertyDescription = listPropertyDescription ?? new List<EntityPropertyDescriptionDataTransfer>();
        }
    }

}
