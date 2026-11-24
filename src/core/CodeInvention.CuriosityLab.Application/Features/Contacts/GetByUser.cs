using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class GetByUser
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
                        .Include(user => user.User)
                        .Include(contactType => contactType.ContactType)
                        .Where(contact => contact.UserId == request.userId)
                        .Select(contact => new Response(contact.Id, contact.User.Name, contact.ContactType.Name, contact.Value))
                        .ToListAsync(cancellationToken);

                return contacts;
            }
        }
        public record Response(int id, string userName, string contactType, string value);
    }
}