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
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.id);

                if(user == null)
                    throw new ArgumentException("User doesn't exists.");

                var response = new Response(user.Id, user.Name, user.Email, user.Age);

                return response;
            }
        }
        public record Response(int id, string name, string email, int? age);
    }
}
