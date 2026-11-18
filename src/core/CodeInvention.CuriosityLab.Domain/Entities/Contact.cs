namespace CodeInvention.CuriosityLab.Domain.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ContactTypeId { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public User User { get; set; }
        public ContactType ContactType { get; set; }
    }
}
