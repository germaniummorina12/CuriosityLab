using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetByLengthName
    {
        public record Query(int length) : IRequest<IEnumerable<Response>>;

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
                    .Where(user => user.Name.Length >= request.length)
                    .Select(user => new Response(user.Id, user.Name, user.Email, user.Age))
                    .ToListAsync(cancellationToken);

                if (users == null)
                    throw new ArgumentException($"No User with longer or equel than {request.length}");

                return users;
            }
        }
        public record Response(int id, string name, string email, int? age);
    }
}
