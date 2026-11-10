using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetUserRoles
    {
        public record Query(int id) : IRequest<Response>;

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Include(user => user.UserRoles)
                        .ThenInclude(role => role.Role)
                    .FirstOrDefaultAsync(user => user.Id == request.id);

                if (user == null)
                    throw new ArgumentException("User doesn't exists.");

                var response = new Response(user.UserRoles
                    .Select(role => role.Role.Name).ToList());

                return response;
            }
        }
        public record Response(List<string> Roles);
    }
}
