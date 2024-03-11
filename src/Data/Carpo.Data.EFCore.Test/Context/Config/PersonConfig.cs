using Carpo.Core.Interface.Domain;
using Carpo.Data.EFCore.Test.Context.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carpo.Data.EFCore.Test.Context.Config
{
    public class PersonConfig : IEntityTypeConfiguration<PersonMap>, IConfigDomain
    {
        public void Configure(EntityTypeBuilder<PersonMap> builder)
        {
            //Primary Key
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id);

            builder.Property(t => t.Name).HasMaxLength(100).IsRequired();

            // Table & Column Mappin
            builder.ToTable("Person", "Clean");
        }
    }
}
