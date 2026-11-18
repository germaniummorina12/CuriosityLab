using CodeInvention.CuriosityLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeInvention.CuriosityLab.Persistence.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("User_Id")
                .IsRequired();

            builder.Property(x => x.ContactTypeId)
                .HasColumnName("Contact_Type_Id")
                .IsRequired();

            builder.Property(x => x.Value)
                .HasColumnName("Value")
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

            builder.ToTable("Contacts");
        }
        private void Constrains(EntityTypeBuilder<Contact> builder)
        {
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }

        private void Relationships(EntityTypeBuilder<Contact> builder)
        {
            builder.HasOne(t => t.User)
                .WithMany(t => t.Contacts)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(t => t.ContactType)
                .WithMany(t => t.Contacts)
                .HasForeignKey(t => t.ContactTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}

