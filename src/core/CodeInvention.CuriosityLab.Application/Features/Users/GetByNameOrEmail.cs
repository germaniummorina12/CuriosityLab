using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CodeInvention.CuriosityLab.Application.Features.Users.GetByEmailDomain;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class GetByNameOrEmail
    {
        public record Query(string text) : IRequest<IEnumerable<Response>>;

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
                    .Include(c => c.Contacts)
                    .Where(user => user.Name.ToLower().Contains(request.text.ToLower()) || 
                        user.Contacts.Any(c => c.ContactTypeId == (int)ContactTypeEnum.Email && 
                        c.Value.ToLower().Contains(request.text.ToLower())))
                    .Select(user => new Response(user.Id,
                        user.Name,
                        user.Contacts
                        .Where(email => email.ContactTypeId == (int)ContactTypeEnum.Email && email.Value.ToLower().Contains(request.text.ToLower()))
                        .Select(email => new EmailResponse(email.Value))
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
