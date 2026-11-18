using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class GetByCity
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
                    .Where(address => address.City != null && address.City.ToLower().Contains(request.city.ToLower()))
                    .Select(address => new Response(address.Id, address.City, address.Street, address.PostalCode))
                    .ToListAsync(cancellationToken);

                return addresses;
            }
        }
        public record Response(int id, string city, string street, string postalCode);
    }
}
