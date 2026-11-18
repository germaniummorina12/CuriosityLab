using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class Update
    {
        public record Command(int id, string name) : IRequest;

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

                if(role == null)
                {
                    throw new ArgumentException("Role not found!");
                }

                role.Name = request.name;
                role.UpdatedAt = DateTime.UtcNow;

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

                RuleFor(role => role.name)
                    .NotEmpty()
                    .WithMessage("Name is Required")
                    .Must((command, name) =>
                    {
                        return !_context.Roles.Any(role => role.Id != command.id && role.Name == name);
                    })
                    .WithMessage("This role already exists");
            }
        }
    }
}
