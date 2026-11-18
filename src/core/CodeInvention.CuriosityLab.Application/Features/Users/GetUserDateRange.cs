using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetUserDateRange
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

                var users = await _context.Users
                    .Where(user => user.CreatedAt >= from && user.CreatedAt <= until)
                    .Select(user => new Response(user.Id, user.Name, user.Age))
                    .ToListAsync(cancellationToken);

                if (users == null)
                    throw new ArgumentException("User doesn't exists.");

                return users;
            }
        }
        public record Response(int id, string name, int? age);
    }
}
