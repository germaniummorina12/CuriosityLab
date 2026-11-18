using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class GetAddressByPostalCode
    {
        public record Query(string postalCode) : IRequest<IEnumerable<Response>>;

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
                    .Where(address => address.PostalCode == request.postalCode)
                    .Select(address => new Response(address.City, address.Street, address.PostalCode))
                    .ToListAsync(cancellationToken);

                return addresses;
            }
        }

        public record Response(string city, string street, string postalCode);
    }
}
