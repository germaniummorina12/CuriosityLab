using CodeInvention.CuriosityLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeInvention.CuriosityLab.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("User_Id")
                .IsRequired();

            builder.Property(x => x.City)
                .HasColumnName("City")
                .IsRequired();

            builder.Property(x => x.Street)
                .HasColumnName("Street")
                .IsRequired();

            builder.Property(x => x.PostalCode)
                .HasColumnName("Postal_Code")
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
            Relationships(builder);

            builder.ToTable("Addresses");
        }
        private void Constrains(EntityTypeBuilder<Address> builder)
        {
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }

        private void Relationships(EntityTypeBuilder<Address> builder)
        {
            builder.HasOne(t => t.User)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
