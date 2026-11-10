using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class Get
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
                    .Select(roles => new Response(roles.Name))
                    .ToListAsync(cancellationToken);

                if(roles == null)
                {
                    throw new ArgumentNullException(nameof(roles));
                }

                return roles;
            }
        }
        public record Response(string name);
    }
}
