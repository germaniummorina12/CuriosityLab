using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
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
                var contact = await _context.Contacts.FirstOrDefaultAsync(contact => contact.Id == request.id);


                if (contact == null)
                {
                    throw new ArgumentException("Contact not found!");
                }

                contact.IsDeleted = true;
                _context.Contacts.Update(contact);
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

                RuleFor(contact => contact.id)
                    .Must((id) =>
                    {
                        return _context.Contacts.Any(contact => contact.Id == id);
                    })
                    .WithMessage("This contact was not found!");
            }
        }
    }
}