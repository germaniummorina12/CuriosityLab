namespace CodeInvention.CuriosityLab.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public User User { get; set; }
    }
}
