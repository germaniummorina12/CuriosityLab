using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class GetUsersWithoutType
    {
        public record Query(int contactTypeId) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await _context.Users
                    .Include(contact => contact.Contacts)
                    .AsNoTracking()
                    .Where(user => !_context.Contacts.Any(contact =>
                        contact.UserId == user.Id &&
                        contact.ContactTypeId == request.contactTypeId))
                    .Select(user => new Response(user.Id, user.Name))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int userId, string userName);
    }
}