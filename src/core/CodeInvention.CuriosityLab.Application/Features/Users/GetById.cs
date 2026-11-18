using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetById
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
                    .Include(u => u.Addresses.Where(a => a.IsDeleted != true))
                    .FirstOrDefaultAsync(u => u.Id == request.id, cancellationToken);

                if (user == null)
                    throw new ArgumentException("User doesn't exist.");

                return new Response(
                    user.Id,
                    user.Name,
                    user.Age,
                    user.Addresses
                        .Select(a => new AddressResponse(a.City, a.Street, a.PostalCode))
                        .ToList()
                );
            }
        }
        public record Response(int id, string name, int? age, List<AddressResponse> addresses);
        public record AddressResponse(string city, string street, string postalcode);
    }
}
