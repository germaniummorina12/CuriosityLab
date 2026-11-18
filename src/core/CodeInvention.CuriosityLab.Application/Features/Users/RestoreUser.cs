using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class RestoreUser
    {
            public record Command(int id) : IRequest;

            public class CommandHandler : IRequestHandler<Command>
            {
                private IApplicationDbContext _context;

                public CommandHandler(IApplicationDbContext context)
                {
                    _context = context;
                }

                public async Task Handle(Command request, CancellationToken cancellationToken)
                {
                    var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(user => user.Id == request.id);

                    if (user == null)
                    {
                        throw new ArgumentException("User doesn't exists");
                    }

                    user.IsDeleted = false;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
