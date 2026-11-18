using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class UpdateAge
    {
        public record Command(int id, int? age) : IRequest;
        public class CommandHandler : IRequestHandler<Command>
        {
            private IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.id);

                if (user == null)
                {
                    throw new ArgumentException("User doesn't exists.", nameof(user));
                }

                user.Age = request.age;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}