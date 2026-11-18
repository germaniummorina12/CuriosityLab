namespace CodeInvention.CuriosityLab.Domain.Entities
{
    public class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
            Addresses = new HashSet<Address>();
            Contacts = new HashSet<Contact>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
