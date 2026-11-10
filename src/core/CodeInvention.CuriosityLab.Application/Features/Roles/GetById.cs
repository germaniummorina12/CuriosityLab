using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetById
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
                var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == request.id);
                if (role == null)
                {
                    throw new ArgumentException("No role found!");
                }
                var response = new Response(role.Name);
                return response;
            }
        }

        public record Response(string name);
    }
}
