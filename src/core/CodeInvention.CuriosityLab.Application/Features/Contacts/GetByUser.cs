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
                // TODO Change this method and start from Users table

                var contacts = await _context.Users
                        .AsNoTracking()
                        .Include(user => user.Contacts)
                            .ThenInclude(user => user.ContactType)
                        .Where(user => user.Id == request.userId)
                        .Select(user => new Response(user.Id, 
                            user.Name, 
                            user.Contacts
                            .Select(contact => new ContactResponse(contact.Id, 
                                contact.ContactType.Name, 
                                contact.Value))
                            .ToList()))
                        .ToListAsync(cancellationToken);

                return contacts;
            }
        }
        public record Response(int id, string userName, IEnumerable<ContactResponse> contacts);
        public record ContactResponse(int id, string contactType, string value);
    }
}