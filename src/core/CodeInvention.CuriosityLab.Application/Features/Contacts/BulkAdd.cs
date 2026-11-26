using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using CodeInvention.CuriosityLab.Domain.Enums;
using FluentValidation;
using MediatR;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class BulkAdd
    {
        public record Item(int contactTypeId, string value);

        public record Command(int userId, List<Item> items) : IRequest
        {
            public IEnumerable<Contact> ToEntities()
            {
                var now = DateTime.UtcNow;

                return items.Select(item => new Contact
                {
                    UserId = userId,
                    ContactTypeId = item.contactTypeId,
                    Value = item.value.Trim().ToLower(),
                    CreatedAt = now
                });
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var contacts = request.ToEntities().ToList();

                _context.Contacts.AddRange(contacts);
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

                RuleFor(command => command.userId)
                    .GreaterThan(0)
                    .WithMessage("User id must be positive")
                    .Must((userId) =>
                    {
                        return _context.Users.Any(user => user.Id == userId);
                    })
                    .WithMessage("No user found with this id!");

                RuleFor(command => command.items)
                    .NotEmpty()
                    .WithMessage("At least one contact is required");

                RuleForEach(command => command.items).ChildRules(item =>
                {
                    item.RuleFor(i => i.value)
                        .NotEmpty()
                        .WithMessage("Value is required")
                        .MaximumLength(255)
                        .WithMessage("Value must be at most 255 characters");

                    item.RuleFor(i => i.contactTypeId)
                        .Must((contactTypeId) =>
                        {
                            return _context.ContactTypes.Any(contactType => contactType.Id == contactTypeId);
                        })
                        .WithMessage("No contact type found with this id!");

                    item.RuleFor(i => i.value)
                        .EmailAddress()
                        .When(i => i.contactTypeId == (int)ContactTypeEnum.Email)
                        .WithMessage("Email is not valid");

                    item.RuleFor(i => i.value)
                        .Matches(@"^\+[1-9]\d{6,14}$")
                        .When(i => i.contactTypeId == (int)ContactTypeEnum.PhoneNumber ||
                                   i.contactTypeId == (int)ContactTypeEnum.FaxNumber)
                        .WithMessage("Phone or fax number is not valid. Use international format, e.g. +3612345678");
                });

                RuleForEach(command => command.items)
                    .Must((command, item) =>
                    {
                        if (string.IsNullOrWhiteSpace(item.value))
                            return true;

                        var value = item.value.Trim().ToLowerInvariant();

                        return !_context.Contacts.Any(contact =>
                            contact.UserId == command.userId &&
                            contact.ContactTypeId == item.contactTypeId &&
                            contact.Value == value);
                    })
                    .WithMessage("This contact has already been added.");
            }
        }
    }
}