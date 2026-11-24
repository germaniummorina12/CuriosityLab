using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class Delete
    {
        public record Command(int id) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == request.id);

                if (role == null)
                {
                    throw new ArgumentException("Role doesn't exists");
                }

                role.IsDeleted = true;
                _context.Roles.Update(role);
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

                RuleFor(role => role.id)
                    .Must((id) =>
                    {
                        return !_context.UserRoles.Any(userRole => userRole.RoleId == id);
                    })
                    .WithMessage("This role is still link to other users");
            }
        }
    }
}