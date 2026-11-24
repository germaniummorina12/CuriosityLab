using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
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
                var contact = await _context.Contacts
                    .Include(user => user.User)
                    .Include(contactType => contactType.ContactType)
                    .FirstOrDefaultAsync(contact => contact.Id == request.id);

                if (contact == null)
                {
                    throw new ArgumentException("Contact not found!");
                }

                return new Response(contact.Id, contact.User.Name, contact.ContactType.Name, contact.Value);
            }
        }
        public record Response(int id, string userName, string contactType, string value);
    }
}
