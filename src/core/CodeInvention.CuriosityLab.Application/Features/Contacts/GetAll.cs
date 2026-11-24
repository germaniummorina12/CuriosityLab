using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class GetAll
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
                var contacts = await _context.Contacts
                    .Include(user => user.User)
                    .Include(contactType => contactType.ContactType)
                    .Select(contact => new Response(contact.Id, contact.User.Name, contact.ContactType.Name, contact.Value))
                    .ToListAsync(cancellationToken);

                return contacts;
            }
        }

        public record Response(int id, string userName, string contactType, string value);
    }
}