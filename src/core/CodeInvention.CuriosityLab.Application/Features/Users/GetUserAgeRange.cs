using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetUserAgeRange
    {
        public record Query(int minAge, int maxAge) : IRequest<IEnumerable<Response>>;

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
                    .Where(user => user.Age != null && user.Age >= request.minAge && user.Age <= request.maxAge)
                    .Select(user => new Response(user.Id, user.Name, user.Age))
                    .ToListAsync(cancellationToken);
               
                return users;
            }
        }
        public record Response(int id, string name, int? age);
    }
}
