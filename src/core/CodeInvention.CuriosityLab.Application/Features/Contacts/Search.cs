using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class Search
    {
        public record Query(string value) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var contacts = await _context.Contacts
                    .Where(contact => contact.Value.ToLower().Contains(request.value.ToLower()))
                    .Select(contact => new Response(contact.Id, contact.Value))
                    .ToListAsync(cancellationToken);

                return contacts;
            }
        }
        public record Response(int id, string value);
    }
}
