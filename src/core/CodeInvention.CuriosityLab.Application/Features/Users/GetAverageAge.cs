

using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetAverageAge
    {
        public record Query() : IRequest<Response>;

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var avg = await _context.Users.AverageAsync(user => user.Age);

                return new Response(avg);
            }
        }

        public record Response(double? avgAge);
    }

}
