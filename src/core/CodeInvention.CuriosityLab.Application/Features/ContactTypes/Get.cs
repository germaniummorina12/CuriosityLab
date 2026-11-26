using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.ContactTypes
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
                var contacts = await _context.ContactTypes
                    .Select(contactType => new Response(contactType.Id, contactType.Name))
                    .ToListAsync(cancellationToken);

                return contacts;
            }
        }

        public record Response(int id, string name);
    }
}