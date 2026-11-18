using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetRoleByName
    {
        public record Query(string input) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await _context.Roles
                    .Where(role => role.Name.ToLower().Contains(request.input.ToLower()))
                    .Select(user => new Response(user.Id, user.Name))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int id, string name);
    }
}
