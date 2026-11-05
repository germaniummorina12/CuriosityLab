namespace CodeInvention.CuriosityLab.Application.Common.Abstractions
{
    public interface IApplicationDbContextInitializer 
    {
        public void Initialize(IApplicationDbContext context);
    }
}
