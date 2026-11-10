using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetByEmailDomain
    {
        public record Query(string input) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var domain = request.input?.Trim().TrimStart('@').ToLower();

                if (string.IsNullOrEmpty(domain))
                {
                    return Enumerable.Empty<Response>();
                }

                var suffix = "@" + domain;

                var users = await _context.Users
                    .Where(user => user.Email.ToLower().Contains(suffix))
                    .Select(user => new Response(user.Id, user.Name, user.Email, user.Age))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int id, string name, string email, int? age);
    }
}
