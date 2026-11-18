using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class GetSpecificCity
    {
        public record Query(string city) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var addresses = await _context.Addresses
                    .Include(user => user.User)
                    .Where(address => address.City.ToLower() == request.city.ToLower())
                       .Select(user => new Response(
                        user.User.Id,
                        user.User.Name,
                        user.User.Age,
                        user.User.Addresses
                            .Select(address => new AddressResponse(
                                address.City,
                                address.Street,
                                address.PostalCode))
                            .ToList()))
                    .ToListAsync(cancellationToken);

                return addresses;
            }
        }

        public record Response(int userId, string userName, int? age, List<AddressResponse> addresses);

        public record AddressResponse(string city, string street, string postalCode);
    }
}