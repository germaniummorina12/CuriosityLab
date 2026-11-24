using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class AddRoleUser
    {
        public record Command(int roleId, int userId) : IRequest
        {
            public UserRole toEntity() => new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == request.roleId);
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.userId);

                if (role == null)
                {
                    throw new ArgumentException("Role not found!");
                }
                if(user == null)
                {
                    throw new ArgumentException("User not found!");
                }
                var assign = request.toEntity();

                _context.UserRoles.Add(assign);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IApplicationDbContext _context;
            public CommandValidator(IApplicationDbContext context)
            {
                _context = context;
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(role => role.roleId)
                    .NotEmpty()
                    .WithMessage("Role is required")
                     .Must((command, roleId) =>
                      {
                          return !_context.UserRoles.Any(role => role.RoleId == roleId && command.userId == role.UserId);
                      })
                      .WithMessage("This role has been assigned to the user before");

                RuleFor(user => user.userId)
                    .NotEmpty()
                    .WithMessage("User is required");
            }
        }
    }
}
