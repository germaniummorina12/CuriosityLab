using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class RemoveRoleUser
    {
        public record Command(int userId, int roleId) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {

                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == request.userId && ur.RoleId == request.roleId, cancellationToken);

                if (userRole == null)
                {
                    throw new ArgumentException("this user doesn’t have this role");
                }


                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
