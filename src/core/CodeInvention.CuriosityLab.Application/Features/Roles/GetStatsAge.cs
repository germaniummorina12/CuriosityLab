using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetStatsAge
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
                var roles = _context.Roles
                    .Include(t => t.UserRoles)
                        .ThenInclude(t => t.User)
                    .Select(role => 
                        new Response(
                            role.Name, 
                            role.UserRoles.Where(t => t.User.Age != null).Min(t => t.User.Age),
                            role.UserRoles.Where(t => t.User.Age != null).Max(t => t.User.Age),
                            role.UserRoles.Where(t => t.User.Age != null).Average(t => t.User.Age)
                        )
                    )
                    .ToList();

                return roles;
            }
        }

        public record Response(string role, int? minAgeUser, int? maxAgeUser, double? avgAgeUser);
    }

}
