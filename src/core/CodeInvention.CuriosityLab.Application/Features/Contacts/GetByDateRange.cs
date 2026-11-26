using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class GetByDateRange
    {
        public record Query(DateTime fromDate, DateTime untilDate) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var fromUtc = DateTime.SpecifyKind(request.fromDate.Date, DateTimeKind.Utc);
                var toUtc = DateTime.SpecifyKind(request.untilDate.Date.AddDays(1), DateTimeKind.Utc);

                var contacts = await _context.Contacts
                    .Include(contact => contact.User)
                    .Include(contact => contact.ContactType)
                    .Where(contact => contact.CreatedAt >= fromUtc && contact.CreatedAt <= toUtc)
                    .Select(contact => new Response(contact.Id, 
                        contact.User.Id, 
                        contact.User.Name, 
                        contact.ContactType.Name, 
                        contact.Value))
                    .ToListAsync(cancellationToken);

                return contacts;
            }
        }
        public record Response(int id, int userId, string userName, string contactTypeName, string value);
    }
}
