using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class GetAddressesByUsers
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
                var addresses = await _context.Addresses
                    .Include(user => user.User)
                    .GroupBy(address => new { address.UserId, address.User.Name })
                    .Select(g => new Response(g.Key.Name, g.Count()))
                    .ToListAsync(cancellationToken);

                return addresses;
            }
        }

        public record Response(string userName, int addressesCount);
    }
}