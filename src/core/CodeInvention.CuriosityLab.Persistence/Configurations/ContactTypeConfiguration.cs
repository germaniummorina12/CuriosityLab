using CodeInvention.CuriosityLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeInvention.CuriosityLab.Persistence.Configurations
{
    public class ContactTypeConfiguration : IEntityTypeConfiguration<ContactType>
    {
        public void Configure(EntityTypeBuilder<ContactType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired(false);

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired(false);

            builder.Property(x => x.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasDefaultValue(false)
                .IsRequired(false);

            Constrains(builder);
            builder.ToTable("ContactTypes");
        }
        private void Constrains(EntityTypeBuilder<ContactType> builder)
        {
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
}
