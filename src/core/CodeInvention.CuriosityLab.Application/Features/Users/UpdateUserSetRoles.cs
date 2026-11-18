using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class UpdateUserSetRoles
    {
        public record Command(int userId, List<int> roleIds) : IRequest; // userId = 1, roleIds = [1,2,5]

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Include(userRole => userRole.UserRoles)
                    .FirstOrDefaultAsync(users => users.Id == request.userId, cancellationToken);

                if (user == null)
                {
                    throw new ArgumentException("User doesn't exist.");
                }

                var roleIds = request.roleIds.Distinct().ToList();

                var roles = await _context.Roles
                    .Where(role => roleIds.Contains(role.Id))
                    .ToListAsync(cancellationToken);

                if (roles.Count != roleIds.Count)
                {
                    throw new ArgumentException("One or more roles don't exist.");
                }

                _context.UserRoles.RemoveRange(user.UserRoles);

                var newUserRoles = request.roleIds // roleIds = [1,2,5]
                    .Select(roleId => new UserRole
                    {
                        UserId = user.Id, // 1, 1, 1
                        RoleId = roleId, // 1, 2, 5
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList();

                _context.UserRoles.AddRange(newUserRoles);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(command => command.roleIds)
                    .NotNull()
                    .WithMessage("Role list is required");
            }
        }
    }
}
