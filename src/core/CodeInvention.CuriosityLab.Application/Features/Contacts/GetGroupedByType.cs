using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class GetGroupedByType
    {
        public record Query(int userId) : IRequest<IEnumerable<Response>>;

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
                    .AsNoTracking()
                    .Where(contact => contact.UserId == request.userId)
                    .Select(contact => new
                    {
                        contact.Id,
                        TypeName = contact.ContactType.Name,
                        contact.Value
                    })
                    .ToListAsync(cancellationToken);

                var grouped = contacts
                    .GroupBy(contact => contact.TypeName)
                    .OrderBy(group => group.Key)
                    .Select(group => new Response(
                        group.Key,
                        group.Select(contact => new Item(contact.Id, contact.Value)).ToList()));

                return grouped.ToList();
            }
        }
        public record Item(int id, string value);

        public record Response(string contactType, List<Item> contacts);
    }
}