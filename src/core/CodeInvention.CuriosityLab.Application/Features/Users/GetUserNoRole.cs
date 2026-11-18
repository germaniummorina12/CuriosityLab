using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetUserNoRole
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
                var users = await _context.Users
                    .Include(user => user.UserRoles)
                    .Where(user => !user.UserRoles.Any())
                    .Select(u => new Response(u.Id, u.Name, u.Email, u.Age))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int id, string name, string email, int? age);
    }
}