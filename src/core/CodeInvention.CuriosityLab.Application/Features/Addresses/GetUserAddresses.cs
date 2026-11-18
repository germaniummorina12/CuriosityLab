using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class GetUserAddresses
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
                var users = await _context.Users
                    .Include(user => user.Addresses)
                    .Where(user => user.Addresses.Count() > 1)
                    .OrderBy(user => user.Id)
                    .Select(user => new Response(
                        user.Id,
                        user.Name,
                        user.Age,
                        user.Addresses
                            .Select(address => new AddressResponse(
                                address.City,
                                address.Street,
                                address.PostalCode))
                            .ToList()))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int userId, string userName, int? age, List<AddressResponse> addresses);

        public record AddressResponse(string city, string street, string postalCode);
    }
}