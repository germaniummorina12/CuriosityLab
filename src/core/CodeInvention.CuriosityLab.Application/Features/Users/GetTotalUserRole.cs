using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetTotalUserRole
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
                var users = await _context.Users
                    .CountAsync(cancellationToken);

                var roles = await _context.Roles
                    .CountAsync(cancellationToken);


                return new Response(users, roles);
            }
        }
        public record Response(int users, int roles);
    }
}
