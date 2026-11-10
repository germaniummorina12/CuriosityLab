using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CodeInvention.CuriosityLab.Application.Features.Roles
{
    public class Add
    {
        public record Command(string name) : IRequest
        {
            public Role toEntity() => new Role
            {
                Name = name,
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
                var role = request.toEntity();
                
                _context.Roles.Add(role);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(role => role.name)
                    .NotEmpty()
                    .WithMessage("Name is Required");
            }
        }
    }
}
