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
