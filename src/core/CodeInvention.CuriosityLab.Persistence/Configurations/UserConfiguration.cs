using CodeInvention.CuriosityLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeInvention.CuriosityLab.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired();

            builder.Property(x => x.Age)
                .HasColumnName("Age")
                .IsRequired(false);

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
            builder.ToTable("Users");
        }

        private void Constrains(EntityTypeBuilder<User> builder)
        {
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
}
