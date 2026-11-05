using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Persistence
{
    public class ApplicationDbContextInitializer : IApplicationDbContextInitializer
    {
        private ApplicationDbContext _context;

        public void Initialize(IApplicationDbContext context)
        {
            _context = (ApplicationDbContext)context;
            _context.Database.Migrate();
        }
    }
}