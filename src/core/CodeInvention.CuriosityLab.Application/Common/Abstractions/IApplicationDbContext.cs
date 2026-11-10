using CodeInvention.CuriosityLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Common.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
