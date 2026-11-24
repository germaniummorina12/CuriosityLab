using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetByEmailDomain
    {
        public record Query(string email) : IRequest<IEnumerable<Response>>;

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var domain = request.email?.Trim().TrimStart('@').ToLower();

                if (string.IsNullOrEmpty(domain))
                {
                    return Enumerable.Empty<Response>();
                }

                var suffix = "@" + domain;

                var users = await _context.Users
                    .Include(c => c.Contacts)
                    .Where(email => email.Contacts.Any(c => c.ContactTypeId == (int)ContactTypeEnum.Email && c.Value.ToLower().Contains(suffix)))
                    .Select(user => new Response(user.Id, 
                        user.Name, 
                        user.Contacts
                        .Where(email => email.ContactTypeId == (int)ContactTypeEnum.Email && email.Value.ToLower().Contains(suffix))
                        .Select(email => new EmailResponse (email.Value))
                        .ToList(), 
                        user.Age))
                    .ToListAsync(cancellationToken);

                return users;
            }
        }
        public record Response(int id, string name, List<EmailResponse> emails, int? age);
        public record EmailResponse(string email);
    }
}
