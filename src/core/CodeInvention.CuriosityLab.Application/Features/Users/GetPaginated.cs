using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetPaginated
    {
        public record Query(int page, int size) : IRequest<Response>;

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }
            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var rowsToSkip = request.page * request.size;

                var users = _context.Users
                    .AsNoTracking()
                    .Include(u => u.Addresses)
                    .Include(u => u.Contacts)
                        .ThenInclude(u => u.ContactType)
                    .Include(u => u.UserRoles)
                        .ThenInclude(u => u.Role)
                    .Select(user => new UserResponse(
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
                            .ToList()));

                var total = await users.CountAsync();
                var result = await users.Skip(rowsToSkip)
                                   .Take(request.size)
                                   .ToListAsync();

                return new Response(total,
                        request.page,
                        request.size,
                        result
                    );
            }
        }
        public record Response(int total, int page, int size, List<UserResponse> users);
        public record UserResponse(int id, string name, int? age, List<AddressResponse> addresses, List<ContactResponse> contacts, List<UserRoleResponse> userRoles);
        public record AddressResponse(string city, string street, string postalcode);
        public record ContactResponse(string contactType, string value);
        public record UserRoleResponse(string name);
    }
}