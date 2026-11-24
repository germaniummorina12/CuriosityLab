using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using CodeInvention.CuriosityLab.Domain.Enums;
using FluentValidation;
using MediatR;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class Add
    {
        public record Command(int userId, int contactTypeId, string value) : IRequest
        {
            public Contact toEntity() => new Contact
            {
                UserId = userId,
                ContactTypeId = contactTypeId,
                Value = value.Trim().ToLower(),
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
                var contact = request.toEntity();

                _context.Contacts.Add(contact);
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

                RuleFor(contact => contact.userId)
                    .NotEmpty()
                    .WithMessage("User id is required");

                RuleFor(contact => contact.contactTypeId)
                    .NotEmpty()
                    .WithMessage("Contact type is required");

                RuleFor(contact => contact.value)
                    .NotEmpty()
                    .WithMessage("Value is required");

                RuleFor(user => user.userId)
                    .Must((userId) =>
                    {
                        return _context.Users.Any(user => user.Id == userId);
                    })
                    .WithMessage("No user found with this id!");

                RuleFor(contactType => contactType.contactTypeId)
                    .Must((contactTypeId) =>
                    {
                        return _context.ContactTypes.Any(contactType => contactType.Id == contactTypeId);
                    })
                    .WithMessage("No contact type found with this id!");

                RuleFor(contact => contact.userId)
                    .Must((command, userId) =>
                    {
                        var value = command.value.Trim().ToLower();

                        return !_context.Contacts.Any(contact => contact.UserId == userId &&
                            command.contactTypeId == contact.ContactTypeId &&
                            contact.Value == value
                            );
                    })
                    .When(command => !string.IsNullOrWhiteSpace(command.value))
                    .WithMessage("This contact has already been added.");
                
                RuleFor(contact => contact.value)
                    .EmailAddress()
                    .When(contact => contact.contactTypeId == (int)ContactTypeEnum.Email)
                    .WithMessage("Email is not valid");

                RuleFor(contact => contact.value)
                    .Matches(@"^\+[1-9]\d{6,14}$")
                    .WithMessage("Phone or fax number is not valid. Use international format, e.g. +3612345678")
                    .When(contact =>
                        contact.contactTypeId == (int)ContactTypeEnum.PhoneNumber ||
                        contact.contactTypeId == (int)ContactTypeEnum.FaxNumber);
            }
        }
    }
}