namespace CodeInvention.CuriosityLab.Application.Common.Models
{
    public class PaginationModel<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public IEnumerable<T> Result { get; set; }

        public PaginationModel(int total, int page, int size, IEnumerable<T> result)
        {
             Total = total;
             Page = page;
             Size = size;
             Result = result;
        }
    }
}
