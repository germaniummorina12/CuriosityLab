using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class GetDateRange
    {
        public record Query(DateTime fromDate, DateTime untilDate) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var from = request.fromDate;
                var until = request.untilDate;

                var addresses = await _context.Addresses
                    .Where(address => address.CreatedAt >= from && address.CreatedAt <= until)
                    .Select(address => new Response(address.Id, address.City, address.Street, address.PostalCode))
                    .ToListAsync(cancellationToken);

                if (addresses == null)
                    throw new ArgumentException("User doesn't exists.");

                return addresses;
            }
        }
        public record Response(int id, string city, string street, string postalCode);
    }
}
