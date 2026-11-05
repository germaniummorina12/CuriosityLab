namespace CodeInvention.CuriosityLab.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CreatedBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
