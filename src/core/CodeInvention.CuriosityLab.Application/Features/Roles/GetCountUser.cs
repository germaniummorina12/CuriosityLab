using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetCountUser
    {
        public record Query() : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roles = await _context.Roles
                    .Include(userRole => userRole.UserRoles)
                    .Select(role =>
                        new Response(role.Name,
                            role.UserRoles.Count()))
                    .ToListAsync(cancellationToken);
                return roles;
            }
        }
        public record Response(string role, int user);
    }
}
