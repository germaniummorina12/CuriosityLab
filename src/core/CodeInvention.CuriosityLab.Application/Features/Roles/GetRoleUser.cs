using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class GetRoleUser
    {
        public record Query(int id) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var role = await _context.Roles
                    .AnyAsync(role => role.Id == request.id, cancellationToken);

                if (!role)
                    throw new ArgumentException("User doesn't exists.");

                var users = await _context.Users
                    .Include(t => t.UserRoles)
                    .Include(t => t.Addresses)
                    .Where(user => user.UserRoles.Any(role => role.RoleId == request.id))
                    .Select(user => new Response(
                        user.Id, 
                        user.Name,
                        user.Age, 
                        user.Addresses
                            .Select(address => new AddressResponse(
                                address.City,
                                address.Street,
                                address.PostalCode))
                            .ToList()
                        )
                    )
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int id, string name, int? age, List<AddressResponse> addresses);
        public record AddressResponse(string? city, string? street, string? postalCode);
    }
}
