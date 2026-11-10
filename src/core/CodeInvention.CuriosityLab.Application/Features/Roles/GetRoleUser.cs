using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetRoleUser
    {
        public record Query(int id) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var role = await _context.Roles
                    .AnyAsync(role => role.Id == request.id, cancellationToken);

                if (!role)
                    throw new ArgumentException("User doesn't exists.");

                var users = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == request.id))
                    .Select(user => new Response(user.Id, user.Name, user.Email, user.Age))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int id, string name, string email, int? age);
    }
}
