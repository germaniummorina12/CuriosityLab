using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetCountUser
    {
        public record Query() : IRequest<Response>;

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var admin = await _context.Users
                    .Include(role => role.UserRoles)
                        .ThenInclude(t => t.Role)
                    .CountAsync(user => user.UserRoles.Any(role => role.Role.Id == 1), cancellationToken);

                var user = await _context.Users
                    .Include(role => role.UserRoles)
                        .ThenInclude(t => t.Role)
                    .CountAsync(user => user.UserRoles.Any(role => role.Role.Id == 2), cancellationToken);
                return new Response(admin, user);
            }
        }
        public record Response(int admin, int user);
    }
}
