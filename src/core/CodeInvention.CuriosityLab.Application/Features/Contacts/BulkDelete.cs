using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
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

                var contacts = await _context.Contacts
                    .Where(contact => contact.IsDeleted != true && ids.Contains(contact.Id))
                    .ToListAsync(cancellationToken);

                if (contacts.Count != ids.Count)
                {
                    throw new ArgumentException("One or more contacts don't exists");
                }

                foreach (var contact in contacts)
                {
                    contact.IsDeleted = true;
                    contact.UpdatedAt = DateTime.UtcNow;
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
                    .WithMessage("At least one contact id is required");

                RuleForEach(command => command.ids)
                    .GreaterThan(0)
                    .WithMessage("Ids must be positive");
            }
        }
    }
}