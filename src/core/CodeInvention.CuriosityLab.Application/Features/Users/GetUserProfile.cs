using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetUserProfile
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
                var user = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Addresses)
                    .Include(u => u.Contacts)
                        .ThenInclude(u => u.ContactType)
                    .Include(u => u.UserRoles)
                        .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == request.id, cancellationToken);

                if (user == null)
                    throw new ArgumentException("User doesn't exist.");

                return new Response(
                    user.Id,
                    user.Name,
                    user.Age,
                    user.Addresses
                        .Where(a => a != null)
                        .Select(a => new AddressResponse(a.City, 
                            a.Street, 
                            a.PostalCode))
                        .ToList(),
                    user.Contacts
                        .Where(c => c != null)
                        .Select(c => new ContactResponse(c.ContactType.Name, 
                            c.Value))
                        .ToList(),
                    user.UserRoles
                        .Where(r => r != null)
                        .Select(r => new UserRoleResponse(r.Role.Name))
                        .ToList()
                );
            }
        }
        public record Response(int id, string name, int? age, List<AddressResponse> addresses, List<ContactResponse> contacts, List<UserRoleResponse> userRoles);
        public record AddressResponse(string city, string street, string postalcode);
        public record ContactResponse(string contactType, string value);
        public record UserRoleResponse(string name);
    }
}
