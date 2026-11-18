using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class BulkDelete
    {
        public record Command(List<int> ids) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var ids = request.ids.Distinct().ToList();

                var users = await _context.Users
                    .Where(user => ids.Contains(user.Id))
                    .ToListAsync(cancellationToken);
                
                if(users.Count != ids.Count)
                {
                    throw new ArgumentException("One or more users don't exists");
                }

                foreach ( var user in users)
                {
                    user.IsDeleted = true;
                    user.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(command => command.ids)
                    .NotEmpty()
                    .WithMessage("At least one user id is required");

                RuleForEach(command => command.ids)
                    .GreaterThan(0)
                    .WithMessage("Ids must be positive");
            }
        }
    }
}
