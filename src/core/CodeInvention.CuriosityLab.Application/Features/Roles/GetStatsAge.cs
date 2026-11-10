using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetStatsAge
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
                var minAgeAdmin = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == 1))
                    .MinAsync(user => user.Age, cancellationToken);

                var minAgeUser = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == 2))
                    .MinAsync(user => user.Age, cancellationToken);

                var maxAgeAdmin = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == 1))
                    .MaxAsync(user => user.Age, cancellationToken);

                var maxAgeUser = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == 2))
                    .MaxAsync(user => user.Age, cancellationToken);

                var avgAgeAdmin = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == 1))
                    .AverageAsync(user => user.Age.Value, cancellationToken);

                var avgAgeUser = await _context.Users
                    .Where(user => user.UserRoles.Any(role => role.RoleId == 2))
                    .AverageAsync(user => user.Age.Value, cancellationToken);

                return new Response(avgAgeAdmin, avgAgeUser, minAgeAdmin, minAgeUser, maxAgeAdmin, maxAgeUser);
            }
        }

        public record Response(double? avgAgeAdmin, double? avgAgeUser, int? minAgeAdmin, int? minAgeUser, int? maxAgeAdmin, int? maxAgeUser);
    }

}
