using Carpo.Core.Interface.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Carpo.Data.EFCore
{
    /// <summary>
    /// Classe base do contexto
    /// </summary>
    public class CarpoContext : DbContext, ICarpoContext
    {
        private ModelBuilder _modelBuilder { get; set; }
        public ModelBuilder ModelBuilder { get { return _modelBuilder; } }

        private string _connectionString { get; set; }
        /// <summary>
        /// Contructor
        /// </summary>        
        public CarpoContext(string nameFile, string nameContext) => _connectionString = Config.ConfigurationDomain.GetConnectionString(nameFile, nameContext); 

        /// <summary>
        /// Constructor
        /// </summary>
        public CarpoContext(string nameContext)
        {
            _connectionString = Config.ConfigurationDomain.GetConnectionString(nameContext);
           // _modelBuilder = this._modelBuilder ?? new ModelBuilder();
        }

        public CarpoContext()
        {
            _connectionString = Config.ConfigurationDomain.GetConnectionString("CarpoContext");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
            configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
            configurationBuilder.Conventions.Remove(typeof(SqlServerOnDeleteConvention));
        }

        /// <summary>
        /// Definir as configurações banco de dados
        /// </summary>        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;

            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.BaseType == null)
                {
                    entity.SetTableName(entity.DisplayName());
                }
            }
 
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            Carpo.Data.EFCore.Config.ConfigurationDomain.SetListConfigurations(modelBuilder);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties().Where(p => p.ClrType == typeof(string))))
            {
                property.SetColumnType("varchar(50)");
            }
        }      
      
    }
}
